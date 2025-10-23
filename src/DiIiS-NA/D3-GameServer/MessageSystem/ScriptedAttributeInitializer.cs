using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;

namespace DiIiS_NA.GameServer.MessageSystem
{
	// exception class for initialization errors
	public class ScriptedAttributeInitializerError : Exception
	{
		public ScriptedAttributeInitializerError() { }
		public ScriptedAttributeInitializerError(string message) : base(message) { }
	}

	// Compiles GameAttribute scripts, generates script dependency lists.
	public class ScriptedAttributeInitializer
	{
		#region Pin() implementation for scripts to use.
		public static int Pin(int a, int b, int c) => b > a ? b : (a > c ? c : a);

		public static float Pin(float a, float b, float c) => b > a ? b : (a > c ? c : a);

		#endregion

		public static void ProcessAttributes(ICollection<GameAttribute> attributes)
		{
			// build string -> GameAttribute lookup
			var attributeLookup = attributes.ToDictionary(attr => attr.Name);
			// will contain C# code for the func<> body that represents each attribute's script.
			var csharpScripts = new Dictionary<GameAttribute, string>();

			// generate C#-compatible source lines from scripts and create attribute dependency lists
			foreach (var attr in attributes.Where(a => !string.IsNullOrEmpty(a.Script)))
			{
				// by default all scripts are not settable
				// can be set to true if self-referring identifier is found
				attr.ScriptedAndSettable = false;

				// replace attribute references with GameAttributeMap lookups
				// also record all attributes used by script into each attribute's dependency list
				var script = Regex.Replace(attr.Script, @"([A-Za-z_]\w*)(\.Agg)?(\#[A-Za-z_]\w*)?(?=[^\(\w]|\z)( \?)?",
					(match) =>
					{
						// lookup attribute object
						string identifierName = match.Groups[1].Value;
						if (!attributeLookup.ContainsKey(identifierName))
							throw new ScriptedAttributeInitializerError("invalid identifer parsed: " + identifierName);

						var identifier = attributeLookup[identifierName];

						// key selection
						int? key = null;
						string keyString = "_key";
						bool usesExplicitKey = false;

						if (match.Groups[3].Success)
						{
                            key = match.Groups[3].Value.ToUpper() switch
                            {
                                "#NONE" => null,
                                "#PHYSICAL" => 0,
                                "#FIRE" => 1,
                                "#LIGHTNING" => 2,
                                "#COLD" => 3,
                                "#POISON" => 4,
                                "#ARCANE" => 5,
                                "#HOLY" => 6,
                                _ => throw new ScriptedAttributeInitializerError("error processing attribute script, invalid key in identifier: " + match.Groups[3].Value),
                            };
                            keyString = (key == null) ? "null" : key.ToString();

							usesExplicitKey = true;
						}

						// add comparsion for int attributes that are directly used in an ?: expression.
						string compare = "";
						if (match.Groups[4].Success)
							compare = identifier is GameAttributeI ? " > 0 ?" : " ?";

						// handle self-referring lookup. example: Resource.Agg
						if (match.Groups[2].Success)
						{
							attr.ScriptedAndSettable = true;
							return "_map._RawGetAttribute(GameAttributes." + identifierName
								+ ", " + keyString + ")" + compare;
						}

						// record dependency
						identifier.Dependents ??= new List<GameAttributeDependency>();

						identifier.Dependents.Add(new GameAttributeDependency(attr, key, usesExplicitKey, false));

						// generate normal lookup
						return "_map[GameAttributes." + identifierName + ", " + keyString + "]" + compare;
					});

				// transform function calls into C# equivalents
				script = Regex.Replace(script, @"floor\(", "(float)Math.Floor(", RegexOptions.IgnoreCase);
				script = Regex.Replace(script, @"max\(", "Math.Max(", RegexOptions.IgnoreCase);
				script = Regex.Replace(script, @"min\(", "Math.Min(", RegexOptions.IgnoreCase);
				script = Regex.Replace(script, @"pin\(", "ScriptedAttributeInitializer.Pin(", RegexOptions.IgnoreCase);

				// add C# single-precision affix to decimal literals. example: 1.25 => 1.25f
				script = Regex.Replace(script, @"\d+\.\d+", "$0f");

				csharpScripts[attr] = script;
			}

			// generate and write final C# code to file
			string sourcePathBase = Path.Combine(Path.GetTempPath(), "DiIiSScriptedAttributeFuncs");
			var formattedScripts = csharpScripts.Select(
                // write out full Func static class field
                s => string.Format(
					"		public static Func<GameAttributeMap, int?, GameAttributeValue> {0} = (_map, _key) => new GameAttributeValue(({1})({2}));",
					s.Key.Name,
                    // select output type cast to ensure it matches attribute type
                    s.Key is GameAttributeF ? "float" : "int",
					s.Value
				)
            
			);

			var codeToCompile = 
@"using System;
using System.Runtime;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;

namespace DiIiS_NA.GameServer.MessageSystem.GeneratedCode
{
	public class ScriptedAttributeFuncs
	{
";
			codeToCompile += string.Join("\r\n", formattedScripts);
			codeToCompile +=
@"	}
}
";

			// compile code
			SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);
			var dd = typeof(Enumerable).GetTypeInfo().Assembly.Location;
			var coreDir = Directory.GetParent(dd);

			MetadataReference[] references = new MetadataReference[]
			{
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
				MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "mscorlib.dll"),
				MetadataReference.CreateFromFile(coreDir.FullName + Path.DirectorySeparatorChar + "System.Runtime.dll"),
				MetadataReference.CreateFromFile(typeof(GameAttributeValue).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(GameAttributeMap).Assembly.Location)
			};

			CSharpCompilation compilation = CSharpCompilation.Create(
				Path.GetRandomFileName(),
				syntaxTrees: new[] { syntaxTree },
				references: references,
				options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

			using (var ms = new MemoryStream())
			{
				EmitResult result = compilation.Emit(ms);

				if (!result.Success)
				{
					StringBuilder emsg = new StringBuilder();
					emsg.AppendLine("encountered errors compiling attribute funcs: ");
					IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
						diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error);

					foreach (Diagnostic diagnostic in failures)
					{
						emsg.AppendLine(string.Format("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage()));
					}

					throw new ScriptedAttributeInitializerError(emsg.ToString());
				}
				else
				{
					ms.Seek(0, SeekOrigin.Begin);

					// pull funcs from new assembly and assign them to their respective attributes
					Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
					Type funcs = assembly.GetType("DiIiS_NA.GameServer.MessageSystem.GeneratedCode.ScriptedAttributeFuncs");
					foreach (var attr in csharpScripts.Keys)
					{
						attr.ScriptFunc = (Func<GameAttributeMap, int?, GameAttributeValue>)funcs
							.GetField(attr.Name).GetValue(null);
					}
				}
			}


		}
	}
}

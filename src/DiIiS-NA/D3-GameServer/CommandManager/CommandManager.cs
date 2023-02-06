using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public static class CommandManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger("CmdMan");
		private static readonly Dictionary<CommandGroupAttribute, CommandGroup> CommandGroups = new Dictionary<CommandGroupAttribute, CommandGroup>();

		static CommandManager()
		{
			RegisterCommandGroups();
		}

		private static void RegisterCommandGroups()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (!type.IsSubclassOf(typeof(CommandGroup))) continue;

				var attributes = (CommandGroupAttribute[])type.GetCustomAttributes(typeof(CommandGroupAttribute), true);
				if (attributes.Length == 0) continue;

				var groupAttribute = attributes[0];
				if (CommandGroups.ContainsKey(groupAttribute))
					Logger.Warn("There exists an already registered command group named '{0}'.", groupAttribute.Name);

				var commandGroup = (CommandGroup)Activator.CreateInstance(type);
				commandGroup.Register(groupAttribute);
				CommandGroups.Add(groupAttribute, commandGroup);
			}
		}

		/// <summary>
		/// Parses a given line from console as a command if any.
		/// </summary>
		/// <param name="line">The line to be parsed.</param>
		public static void Parse(string line)
		{
			string output = string.Empty;
			var found = false;

			if (line == null) return;
			if (line.Trim() == string.Empty) return;

			if (!ExtractCommandAndParameters(line, out var command, out var parameters))
			{
				output = "Unknown command: " + line;
				Logger.Error(output);
				return;
			}

			foreach (var pair in CommandGroups.Where(pair => pair.Key.Name == command))
			{
				output = pair.Value.Handle(parameters);
				found = true;
				break;
			}

			if (found == false)
			{
				Logger.Error("Unknown command: " + command);
				return;
			}

			Logger.Success(output != string.Empty ? output : "Command executed successfully.");
		}


		/// <summary>
		/// Tries to parse given line as a server command.
		/// </summary>
		/// <param name="line">The line to be parsed.</param>
		/// <param name="invokerClient">The invoker client if any.</param>
		/// <returns><see cref="bool"/></returns>
		public static bool TryParse(string line, BattleClient invokerClient)
		{
			string output = string.Empty;
			string command;
			string parameters;
			var found = false;

			if (invokerClient == null)
				throw new ArgumentException("invokerClient");

			if (!ExtractCommandAndParameters(line, out command, out parameters))
				return false;

			foreach (var pair in CommandGroups)
			{
				if (pair.Key.Name != command) continue;
				output = pair.Value.Handle(parameters, invokerClient);
				found = true;
				break;
			}

			if (found == false)
				output = $"Unknown command: {command} {parameters}";

			if (output == string.Empty)
				return true;

			if (output.Contains("\n"))
			{
				invokerClient.SendServerWhisper("[SYSTEM]\n" + output + "\n\n");
			}
			else
			{
				invokerClient.SendServerWhisper("[SYSTEM] " + output);
			}
			return true;
		}

		public static bool ExtractCommandAndParameters(string line, out string command, out string parameters)
		{
			line = line.Trim();
			command = string.Empty;
			parameters = string.Empty;

			if (line == string.Empty)
				return false;

			if (line[0] != CommandsConfig.Instance.CommandPrefix) // if line does not start with command-prefix
				return false;

			line = line.Substring(1); // advance to actual command.
			command = line.Split(' ')[0].ToLower(); // get command
			parameters = String.Empty;
			if (line.Contains(' ')) parameters = line.Substring(line.IndexOf(' ') + 1).Trim(); // get parameters if any.

			return true;
		}

		[CommandGroup("commands", "Lists available commands for your user-level.")]
		public class CommandsCommandGroup : CommandGroup
		{
			public override string Fallback(string[] parameters = null, BattleClient invokerClient = null)
			{
				var output = "Available commands: ";
				output = 
					invokerClient != null 
						? CommandGroups.Where(pair => pair.Key.MinUserLevel > invokerClient?.Account.UserLevel)
							.Aggregate(output, (current, pair) => current + ($"{CommandsConfig.Instance.CommandPrefix}{pair.Key.Name}: {pair.Key.Help}\n\n")) 
						: CommandGroups
							.Aggregate(output, (current, pair) => current + (($"$[underline green]${CommandsConfig.Instance.CommandPrefix}{pair.Key.Name}$[/]$: {pair.Key.Help}\n\n")));

				return output + "Type 'help <command>' to get help about a specific command.";
			}
		}

		[CommandGroup("help", "usage: help <command>\nType 'commands' to get a list of available commands.")]
		public class HelpCommandGroup : CommandGroup
		{
			public override string Fallback(string[] parameters = null, BattleClient invokerClient = null) => "usage: help <command>\nType 'commands' to get a list of available commands.";

			public override string Handle(string parameters, BattleClient invokerClient = null)
			{
				if (parameters == string.Empty)
					return Fallback();

				string output = string.Empty;
				bool found = false;
				var @params = parameters.Split(' ');
				var group = @params[0];
				var command = @params.Length > 1 ? @params[1] : string.Empty;

				foreach (var pair in CommandGroups.Where(pair => group == pair.Key.Name))
				{
					if (command == string.Empty)
						return pair.Key.Help;

					output = pair.Value.GetHelp(command);
					found = true;
				}

				if (!found)
					output = $"Unknown command: {group} {command}";

				return output;
			}
		}
	}
}

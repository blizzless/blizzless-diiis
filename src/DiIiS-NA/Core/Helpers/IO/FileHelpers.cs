//Blizzless Project 2022
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;

namespace DiIiS_NA.Core.Helpers.IO
{
	public static class FileHelpers
	{
		public static string AssemblyRoot
		{
			get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
		}

		public static List<string> GetFilesByExtensionRecursive(string directory, string fileExtension)
		{
			var files = new List<string>(); 
			var stack = new Stack<string>();

			stack.Push(directory); 

			while (stack.Count > 0)
			{
				var topDir = stack.Pop();
				var dirInfo = new DirectoryInfo(topDir);

				files.AddRange((from fileInfo in dirInfo.GetFiles()
								where string.Compare(fileInfo.Extension, fileExtension, System.StringComparison.OrdinalIgnoreCase) == 0
								select topDir + "/" + fileInfo.Name).ToList());

				foreach (var dir in Directory.GetDirectories(topDir)) 
				{
					stack.Push(dir);
				}
			}

			return files.Select(file => file.Replace("\\", "/")).ToList();
		}
	}
}

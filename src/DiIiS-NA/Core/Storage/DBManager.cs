//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using Microsoft.Data.Sqlite;
//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.Core.Storage
{
	public static class DBManager
	{
		public static SqliteConnection MPQMirror { get; private set; }

		public static readonly Logger Logger = LogManager.CreateLogger("DB");

		public static string AssetDirectory
		{
			get
			{
				var dataDirectory = String.Format(@"{0}/{1}", FileHelpers.AssemblyRoot, Config.Instance.Root);

				if (Path.IsPathRooted(Config.Instance.Root))
					//Path is rooted... dont use assemblyRoot, as its absolute path.
					dataDirectory = Config.Instance.Root;
				return dataDirectory;
			}
		}

		static DBManager()
		{
			Connect();
		}

		private static void Connect()
		{
			try
			{

				MPQMirror = new SqliteConnection(String.Format("Data Source={0}/mpqdata.db", AssetDirectory));
				MPQMirror.Open();
			}
			catch (Exception e)
			{
				Logger.FatalException(e, "Connect()");
			}
		}
	}
}

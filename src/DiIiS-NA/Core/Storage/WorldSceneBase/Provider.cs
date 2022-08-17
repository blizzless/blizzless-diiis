//Blizzless Project 2022 
using System.IO;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using FluentNHibernate;
//Blizzless Project 2022 
using FluentNHibernate.Cfg;
//Blizzless Project 2022 
using NHibernate;
//Blizzless Project 2022 
using NHibernate.Cfg;
//Blizzless Project 2022 
using NHibernate.Tool.hbm2ddl;
//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.IO;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Mapper;

namespace DiIiS_NA.Core.Storage.WorldSceneBase
{
	public class Provider
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _config;
		private static readonly object Lockobj = new object();
		public static ISessionFactory SessionFactory
		{
			get
			{
				lock (Lockobj)
				{
					return _sessionFactory ?? (_sessionFactory = CreateSessionFactory());
				}
			}
		}


		public static Configuration Config
		{
			get
			{
				if (_config == null)
				{
					_config = new Configuration();
					_config = _config.Configure(Path.Combine(FileHelpers.AssemblyRoot, "database.Worlds.config"));


					var replacedProperties = new Dictionary<string, string>();
					foreach (var prop in _config.Properties)
					{
						var newvalue = prop.Value;
						newvalue = newvalue.Replace("{$ASSETBASE}", DBManager.AssetDirectory);
						replacedProperties.Add(prop.Key, newvalue);
					}


					_config = _config.SetProperties(replacedProperties);
					_config = _config.AddMappingsFromAssembly(Assembly.GetAssembly(typeof(DBAccountMapper)));
					if (_config.Properties.ContainsKey("dialect"))
						if (_config.GetProperty("dialect").ToLower().Contains("sqlite"))
							_config = _config.SetProperty("connection.release_mode", "on_close");
				}

				return _config;
			}
		}

		private static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure(Config).ExposeConfiguration(
				cfg =>
					new SchemaUpdate(cfg).Execute(true, true)
				).
				BuildSessionFactory();
		}

		public static void RebuildSchema()
		{
			var schema = new SchemaUpdate(Config);
			schema.Execute(true, true);
		}
	}
}

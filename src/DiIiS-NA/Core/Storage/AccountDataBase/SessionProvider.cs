using System.IO;
using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using DiIiS_NA.Core.Helpers.IO;
using DiIiS_NA.Core.Storage.WorldSceneBase.Mapper;

namespace DiIiS_NA.Core.Storage.AccountDataBase
{
	public class SessionProvider
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
#if DEBUG
                    _config = _config.Configure(File.Exists(Path.Combine(FileHelpers.AssemblyRoot, "database.Account.Debug.config")) ? Path.Combine(FileHelpers.AssemblyRoot, "database.Account.Debug.config") : Path.Combine(FileHelpers.AssemblyRoot, "database.Account.config"));
#else
                    _config = _config.Configure(Path.Combine(FileHelpers.AssemblyRoot, "database.Account.config"));
#endif

                    var replacedProperties = new Dictionary<string, string>();
					foreach (var prop in _config.Properties)
					{
						var newvalue = prop.Value;
						newvalue = newvalue.Replace("{$ASSETBASE}", DBManager.AssetDirectory);
						replacedProperties.Add(prop.Key, newvalue);
					}
					var dic = new Dictionary<string, string>();
					dic.Add("dialect", "DiIiS_NA.Core.Storage.PostgresDialect");
					//var Dial = NHibernate.Dialect.Dialect.GetDialect(dic);
					
					_config = _config.SetProperties(replacedProperties);
					_config = _config.AddMappingsFromAssembly(Assembly.GetAssembly(typeof(DRLG_ContainerMapper)));
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

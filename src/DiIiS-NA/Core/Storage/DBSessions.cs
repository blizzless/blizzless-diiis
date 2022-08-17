//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using Microsoft.Data.Sqlite;
//Blizzless Project 2022 
using NHibernate;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using NHibernate.Linq;

namespace DiIiS_NA.Core.Storage
{
	public static class DBSessions
	{
		private static Object _sessionLock = new object();
		private static IStatelessSession _worldSession = null;

		private static readonly Logger Logger = LogManager.CreateLogger("DB");

		public static IStatelessSession WorldSession
		{
			get
			{
				lock (_sessionLock)
				{
					if (_worldSession == null || !_worldSession.IsOpen)
					{
						_worldSession = WorldSceneBase.Provider.SessionFactory.OpenStatelessSession();
					}
				}
				return _worldSession;
			}
		}

		public static void SessionSave(Object obj)
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					session.Insert(obj);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static void SessionUpdate(Object obj)
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					session.Update(obj);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static void SessionDelete(Object obj)
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					session.Delete(obj);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static List<T> SessionQueryWhereContains<T>(System.Linq.Expressions.Expression<Func<T, object>> expression, List<ulong> list) where T : class
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					return session.QueryOver<T>().WhereRestrictionOn(expression).IsIn(list).List().ToList();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static List<T> SessionQuery<T>()
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					return session.Query<T>().ToList();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static List<T> SessionQueryWhere<T>(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate) where T : class
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					return session.QueryOver<T>().Where(predicate).List().ToList();
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static T SessionQuerySingle<T>(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate) where T : class
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					return session.Query<T>().Single(predicate);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}

		public static T SessionGet<T>(Object obj)
		{
			try
			{
				//Blizzless Project 2022 
using (IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession())
					return (T)session.Get<T>(obj);
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "Unhandled DB exception caught:");
				throw;
			}
		}
	}
}

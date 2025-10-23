using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NHibernate;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;

namespace DiIiS_NA.Core.Storage
{
	public class GameDBSession
	{
		private static Object _globalSessionLock = new object();
		private Object _sessionLock = new object();
		private IStatelessSession _gameSession = null;
		private readonly Logger Logger = LogManager.CreateLogger(nameof(GameDBSession));

		public GameDBSession()
		{
			lock (_globalSessionLock)
			{
				_gameSession = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
			}
		}

		private IStatelessSession GameSession => _gameSession;

		public void SessionSave(Object obj)
		{
			Task.Run(() =>
			{
				if (obj is DBAchievements)
				{
					try
					{
						using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
						session.Insert(obj);
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
				else
				{
					lock (_sessionLock)
					{
						try
						{
							GameSession.Insert(obj);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "Unhandled DB exception caught:");
							throw;
						}
					}
				}
			});
		}

		public void SessionUpdate(Object obj)
		{
			Task.Run(() =>
			{
				if (obj is DBAchievements)
				{
					try
					{
						using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
						session.Update(obj);
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
				else
				{
					lock (_sessionLock)
					{
						try
						{
							GameSession.Update(obj);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "Unhandled DB exception caught:");
							throw;
						}
					}
				}
			});
		}

		public void SessionDelete(Object obj)
		{
			Task.Run(() =>
			{
				if (obj is DBAchievements)
				{
					try
					{
						using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
						session.Delete(obj);
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
				else
				{
					lock (_sessionLock)
					{
						try
						{
							GameSession.Delete(obj);
						}
						catch (Exception e)
						{
							Logger.WarnException(e, "Unhandled DB exception caught:");
							throw;
						}
					}
				}
			});
		}

		public void SessionDispose()
		{
			lock (_sessionLock)
			{
				try
				{
					GameSession.Dispose();
				}
				catch
				{
					Logger.Debug("DB exception! Can't Dispose");
				}
			}
		}

		public List<T> SessionQuery<T>()
		{
			if (typeof(T) == typeof(DBAchievements))
			{
				try
				{
					using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
					return session.Query<T>().ToList();
				}
				catch (Exception e)
				{
					Logger.WarnException(e, "Unhandled DB exception caught:");
					throw;
				}
			}
			else
			{
				lock (_sessionLock)
				{
					try
					{
						return GameSession.Query<T>().ToList();
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
			}
		}

		public List<T> SessionQueryWhere<T>(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate) where T : class
		{
			if (typeof(T) == typeof(DBAchievements))
			{
				try
				{
					using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
					return session.QueryOver<T>().Where(predicate).List().ToList();
				}
				catch (Exception e)
				{
					Logger.WarnException(e, "Unhandled DB exception caught:");
					throw;
				}
			}
			else
			{
				lock (_sessionLock)
				{
					try
					{
						return GameSession.QueryOver<T>().Where(predicate).List().ToList();
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
			}
		}

		public T SessionQuerySingle<T>(System.Linq.Expressions.Expression<System.Func<T, bool>> predicate) where T : class
		{
			if (typeof(T) == typeof(DBAchievements))
			{
				try
				{
					using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
					return (T)session.QueryOver<T>().Where(predicate).List().FirstOrDefault();
				}
				catch (Exception e)
				{
					Logger.WarnException(e, "Unhandled DB exception caught:");
					throw;
				}
			}
			else
			{
				lock (_sessionLock)
				{
					try
					{
						return (T)GameSession.QueryOver<T>().Where(predicate).List().FirstOrDefault();
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
			}
		}

		public void Update<T>(object obj, Action<T> execute)
		{
			using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
			var db = session.Get<T>(obj);
			execute(db);
			session.Update(db);
		}
		
		public T SessionGet<T>(Object obj)
		{
			if (typeof(T) == typeof(DBAchievements))
			{
				try
				{
					using IStatelessSession session = AccountDataBase.SessionProvider.SessionFactory.OpenStatelessSession();
					return session.Get<T>(obj);
				}
				catch (Exception e)
				{
					Logger.WarnException(e, "Unhandled DB exception caught:");
					throw;
				}
			}
			else
			{
				lock (_sessionLock)
				{
					try
					{
						return (T)GameSession.Get<T>(obj);
					}
					catch (Exception e)
					{
						Logger.WarnException(e, "Unhandled DB exception caught:");
						throw;
					}
				}
			}
		}
	}
}

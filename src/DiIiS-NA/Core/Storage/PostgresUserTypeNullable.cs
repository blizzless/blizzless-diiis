//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Data.Common;
//Blizzless Project 2022 
using NHibernate;
//Blizzless Project 2022 
using NHibernate.Engine;
//Blizzless Project 2022 
using NHibernate.SqlTypes;
//Blizzless Project 2022 
using NHibernate.UserTypes;

namespace DiIiS_NA.Core.Storage
{
	public class PostgresUserTypeNullable : IUserType
	{
		public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			var value = NHibernateUtil.Int64.NullSafeGet(rs, names[0], session);
			return (value == null) ? 0 : Convert.ToUInt64(value);
		}

		public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			//object valueToSet = (value == null || Convert.ToInt64(value) == 0) ? DBNull.Value : (object)Convert.ToInt64(value);
			if (value == null || Convert.ToInt64(value) == 0)
				NHibernateUtil.Int64.NullSafeSet(cmd, null, index, session);
			else
				NHibernateUtil.Int64.NullSafeSet(cmd, Convert.ToInt64(value), index, session);
			//cmd.Parameters[index] = valueToSet;
			//NHibernateUtil.Int64.NullSafeSet(cmd, valueToSet, index);
		}

		public Type ReturnedType
		{
			get { return typeof(UInt64); }
		}

		public SqlType[] SqlTypes
		{
			get { return new[] { SqlTypeFactory.Int64 }; }
		}

		public new bool Equals(object x, object y)
		{
			if (x == y) return true;

			ulong lhs = (x == null) ? 0 : (ulong)x;
			ulong rhs = (y == null) ? 0 : (ulong)y;
			return lhs.Equals(rhs);
		}

		public int GetHashCode(object x)
		{
			return (x == null) ? 0 : x.GetHashCode();
		}

		public object DeepCopy(object value)
		{
			return value;
		}

		public bool IsMutable
		{
			get { return false; }
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return cached;
		}

		public object Disassemble(object value)
		{
			return value;
		}
	}
}

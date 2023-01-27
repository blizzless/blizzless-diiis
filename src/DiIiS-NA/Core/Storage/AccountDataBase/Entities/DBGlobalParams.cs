using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGlobalParams : Entity
	{
		public new virtual ulong Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ulong Value { get; set; }
	}
}

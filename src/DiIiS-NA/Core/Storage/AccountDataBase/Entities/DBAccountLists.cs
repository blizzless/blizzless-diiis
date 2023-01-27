using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBAccountLists : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBAccount ListOwner { get; set; }
		public virtual DBAccount ListTarget { get; set; }
		public virtual string Type { get; set; }
	}
}

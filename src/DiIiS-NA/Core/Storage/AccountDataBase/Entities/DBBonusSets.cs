//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBBonusSets : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual int SetId { get; set; }
		public virtual DBAccount DBAccount { get; set; }
		public virtual bool Claimed { get; set; }
		public virtual DBToon ClaimedToon { get; set; }
		public virtual bool ClaimedHardcore { get; set; }
	}
}

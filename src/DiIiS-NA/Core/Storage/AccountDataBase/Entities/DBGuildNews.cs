using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGuildNews : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBGuild DBGuild { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual int Type { get; set; }
		public virtual ulong Time { get; set; }
		public virtual byte[] Data { get; set; }
	}
}

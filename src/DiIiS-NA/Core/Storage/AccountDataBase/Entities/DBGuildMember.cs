//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGuildMember : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBGuild DBGuild { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual string Note { get; set; }
		public virtual int Rank { get; set; }
	}
}

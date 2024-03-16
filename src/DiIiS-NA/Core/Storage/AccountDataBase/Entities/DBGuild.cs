using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGuild : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual string Name { get; set; }
		public virtual string Tag { get; set; }
		public virtual string Description { get; set; }
		public virtual string MOTD { get; set; }
		public virtual int Category { get; set; }
		public virtual int Language { get; set; }
		public virtual bool IsLFM { get; set; }
		public virtual bool IsInviteRequired { get; set; }
		public virtual int Rating { get; set; }
		public virtual DBGameAccount Creator { get; set; }
		public virtual byte[] Ranks { get; set; }
		public virtual bool Disbanded { get; set; }
	}
}

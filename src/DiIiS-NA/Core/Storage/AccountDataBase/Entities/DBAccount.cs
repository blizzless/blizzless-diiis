//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using FluentNHibernate.Data;


namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBAccount : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual string Email { get; set; }
		public virtual bool Banned { get; set; }
		public virtual byte[] Salt { get; set; }
		public virtual byte[] PasswordVerifier { get; set; }
		public virtual string BattleTagName { get; set; }
		public virtual string SaltedTicket { get; set; }
		public virtual int HashCode { get; set; }
		public virtual int ReferralCode { get; set; }
		public virtual DBAccount InviteeAccount { get; set; }
		public virtual ulong Money { get; set; }
		public virtual Account.UserLevels UserLevel { get; set; }
		public virtual ulong LastOnline { get; set; }
		public virtual bool HasRename { get; set; }
		public virtual ulong RenameCooldown { get; set; }
        public virtual ulong DiscordId { get;  set; }
        public virtual string DiscordTag { get; set; }
    }
}

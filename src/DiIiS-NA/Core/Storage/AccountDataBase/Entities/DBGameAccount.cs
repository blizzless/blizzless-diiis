//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGameAccount : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBAccount DBAccount { get; set; }
		public virtual byte[] Banner { get; set; }
		public virtual byte[] UIPrefs { get; set; }
		public virtual byte[] UISettings { get; set; }
		public virtual byte[] SeenTutorials { get; set; }
		public virtual byte[] BossProgress { get; set; }
		public virtual byte[] StashIcons { get; set; }
		public virtual int Flags { get; set; }
		public virtual int ParagonLevel { get; set; }
		public virtual int ParagonLevelHardcore { get; set; }
		public virtual long Experience { get; set; }
		public virtual long ExperienceHardcore { get; set; }
		public virtual ulong LastOnline { get; set; }
		public virtual DBToon LastPlayedHero { get; set; }
		public virtual ulong Gold { get; set; }
		public virtual int Platinum { get; set; }
		public virtual ulong HardcoreGold { get; set; }
		public virtual ulong RmtCurrency { get; set; }
		public virtual int BloodShards { get; set; }
		public virtual int HardcoreBloodShards { get; set; }
		public virtual int StashSize { get; set; }
		public virtual int HardcoreStashSize { get; set; }
		public virtual int SeasonStashSize { get; set; }
		public virtual ulong ElitesKilled { get; set; }
		public virtual ulong TotalKilled { get; set; }
		public virtual ulong TotalGold { get; set; }
		public virtual int TotalBloodShards { get; set; }
		public virtual int TotalBounties { get; set; }
		public virtual int TotalBountiesHardcore { get; set; }
		public virtual ulong PvPTotalKilled { get; set; }
		public virtual ulong PvPTotalWins { get; set; }
		public virtual ulong PvPTotalGold { get; set; }
	}
}

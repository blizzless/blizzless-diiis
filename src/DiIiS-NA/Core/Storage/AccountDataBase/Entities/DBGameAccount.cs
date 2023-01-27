using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBGameAccount : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBAccount DBAccount { get; set; }
		public virtual ulong LastOnline { get; set; }
		public virtual DBToon LastPlayedHero { get; set; }
		public virtual int Flags { get; set; }
		public virtual byte[] Banner { get; set; }
		public virtual byte[] UIPrefs { get; set; }
		public virtual byte[] UISettings { get; set; }
		public virtual byte[] SeenTutorials { get; set; }
		public virtual byte[] BossProgress { get; set; }
		public virtual byte[] StashIcons { get; set; }
		public virtual int ParagonLevel { get; set; }
		public virtual int ParagonLevelHardcore { get; set; }
		public virtual long Experience { get; set; }
		public virtual long ExperienceHardcore { get; set; }
		public virtual ulong Gold { get; set; }
		public virtual int Platinum { get; set; }
		public virtual int HardPlatinum { get; set; }
		public virtual ulong HardcoreGold { get; set; }
		public virtual ulong RmtCurrency { get; set; }
		public virtual ulong HardRmtCurrency { get; set; }
		public virtual int BloodShards { get; set; }
		public virtual int HardcoreBloodShards { get; set; }
		public virtual int StashSize { get; set; }
		public virtual int HardcoreStashSize { get; set; }
		public virtual int SeasonStashSize { get; set; }
		public virtual int HardSeasonStashSize { get; set; }
		public virtual ulong ElitesKilled { get; set; }
		public virtual ulong HardElitesKilled { get; set; }
		public virtual ulong TotalKilled { get; set; }
		public virtual ulong HardTotalKilled { get; set; }
		public virtual ulong TotalGold { get; set; }
		public virtual ulong HardTotalGold { get; set; }
		public virtual int TotalBloodShards { get; set; }
		public virtual int HardTotalBloodShards { get; set; }
		public virtual int TotalBounties { get; set; }
		public virtual int TotalBountiesHardcore { get; set; }
		public virtual ulong PvPTotalKilled { get; set; }
		public virtual ulong HardPvPTotalKilled { get; set; }
		public virtual ulong PvPTotalWins { get; set; }
		public virtual ulong HardPvPTotalWins { get; set; }
		public virtual ulong PvPTotalGold { get; set; }
		public virtual ulong HardPvPTotalGold { get; set; }
		public virtual int CraftItem1 { get; set; }
		public virtual int HardCraftItem1 { get; set; }
		public virtual int CraftItem2 { get; set; }
		public virtual int HardCraftItem2 { get; set; }
		public virtual int CraftItem3 { get; set; }
		public virtual int HardCraftItem3 { get; set; }
		public virtual int CraftItem4 { get; set; }
		public virtual int HardCraftItem4 { get; set; }
		public virtual int CraftItem5 { get; set; }
		public virtual int HardCraftItem5 { get; set; }
		public virtual int BigPortalKey { get; set; }
		public virtual int HardBigPortalKey { get; set; }
		public virtual int LeorikKey { get; set; }
		public virtual int HardLeorikKey { get; set; }
		public virtual int VialofPutridness { get; set; }
		public virtual int HardVialofPutridness { get; set; }
		public virtual int IdolofTerror { get; set; }
		public virtual int HardIdolofTerror { get; set; }
		public virtual int HeartofFright { get; set; }
		public virtual int HardHeartofFright { get; set; }
		public virtual int HoradricA1 { get; set; }
		public virtual int HardHoradricA1 { get; set; }
		public virtual int HoradricA2 { get; set; }
		public virtual int HardHoradricA2 { get; set; }
		public virtual int HoradricA3 { get; set; }
		public virtual int HardHoradricA3 { get; set; }
		public virtual int HoradricA4 { get; set; }
		public virtual int HardHoradricA4 { get; set; }
		public virtual int HoradricA5 { get; set; }
		public virtual int HardHoradricA5 { get; set; }
	}
}

//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022 
using FluentNHibernate.Data;
//Blizzless Project 2022 
using static DiIiS_NA.LoginServer.Toons.Toon;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBToon : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual string Name { get; set; }
		public virtual ToonClass Class { get; set; }
		public virtual ToonFlags Flags { get; set; }
		public virtual byte Level { get; set; }

		public virtual bool Dead { get; set; }
		public virtual bool StoneOfPortal { get; set; }
		public virtual int CreatedSeason { get; set; }
		public virtual int TimeDeadHarcode { get; set; }

		public virtual long Experience { get; set; }
		public virtual ushort[] ParagonBonuses { get; set; }
		public virtual int PvERating { get; set; }
		public virtual int ChestsOpened { get; set; }                   
		public virtual int EventsCompleted { get; set; }
		public virtual int Kills { get; set; }
		public virtual int Deaths { get; set; }
		public virtual int ElitesKilled { get; set; }
		public virtual int GoldGained { get; set; }
		public virtual int? ActiveHireling { get; set; }
		public virtual bool isHardcore { get; set; }
		public virtual bool isSeasoned { get; set; }
		public virtual int CurrentAct { get; set; }
		public virtual int CurrentQuestId { get; set; }
		public virtual int CurrentQuestStepId { get; set; }
		public virtual int CurrentDifficulty { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual int TimePlayed { get; set; }
		public virtual string Stats { get; set; }
		public virtual byte[] Lore { get; set; }
		public virtual bool Deleted { get; set; }
		public virtual bool Archieved { get; set; }
		public virtual int WingsActive { get; set; }
		public virtual int Cosmetic1 { get; set; }
		public virtual int Cosmetic2 { get; set; }
		public virtual int Cosmetic3 { get; set; }
		public virtual int Cosmetic4 { get; set; }
	}
}

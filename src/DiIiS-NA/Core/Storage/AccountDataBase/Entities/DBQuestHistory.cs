//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBQuestHistory : Entity
	{
		public new virtual ulong Id { get; set; }
		public virtual DBToon DBToon { get; set; }
		public virtual int QuestId { get; set; }
		public virtual bool isCompleted { get; set; }
		public virtual int QuestStep { get; set; }
	}
}

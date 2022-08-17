//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBAchievements : Entity
	{
		public new virtual ulong Id { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual ulong AchievementId { get; set; }
		public virtual int CompleteTime { get; set; }
		public virtual int Quantity { get; set; }
		public virtual bool IsHardcore { get; set; }
		public virtual byte[] Criteria { get; set; }
	}
}

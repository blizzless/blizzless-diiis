//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBCraft : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual bool isHardcore { get; set; }
		public virtual bool isSeasoned { get; set; }
		public virtual string Artisan { get; set; }
		public virtual int Level { get; set; }
		public virtual byte[] LearnedRecipes { get; set; }
	}
}

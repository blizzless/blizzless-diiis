//Blizzless Project 2022 
using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBReport : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual string Type { get; set; }
		public virtual DBGameAccount Sender { get; set; }
		public virtual DBGameAccount DBGameAccount { get; set; }
		public virtual DBToon DBToon { get; set; }
		public virtual string Note { get; set; }
	}
}

using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBMail : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBToon DBToon { get; set; }
		public virtual bool Claimed { get; set; }
		public virtual string Title { get; set; }
		public virtual string Body { get; set; }
		public virtual int ItemGBID { get; set; }
	}
}

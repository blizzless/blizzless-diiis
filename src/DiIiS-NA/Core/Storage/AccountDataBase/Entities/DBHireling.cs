using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBHireling : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBToon DBToon { get; set; }
		public virtual int Class { get; set; }
		public virtual int Skill1SNOId { get; set; }
		public virtual int Skill2SNOId { get; set; }
		public virtual int Skill3SNOId { get; set; }
		public virtual int Skill4SNOId { get; set; }
	}
}

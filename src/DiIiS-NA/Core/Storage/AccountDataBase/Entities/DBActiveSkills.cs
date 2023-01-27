using FluentNHibernate.Data;

namespace DiIiS_NA.Core.Storage.AccountDataBase.Entities
{
	public class DBActiveSkills : Entity
	{
		public new virtual ulong Id { get; protected set; }
		public virtual DBToon DBToon { get; set; }
		public virtual int Skill0 { get; set; }
		public virtual int Rune0 { get; set; }
		public virtual int Skill1 { get; set; }
		public virtual int Rune1 { get; set; }
		public virtual int Skill2 { get; set; }
		public virtual int Rune2 { get; set; }
		public virtual int Skill3 { get; set; }
		public virtual int Rune3 { get; set; }
		public virtual int Skill4 { get; set; }
		public virtual int Rune4 { get; set; }
		public virtual int Skill5 { get; set; }
		public virtual int Rune5 { get; set; }

		public virtual int Passive0 { get; set; }
		public virtual int Passive1 { get; set; }
		public virtual int Passive2 { get; set; }
		public virtual int Passive3 { get; set; }

		public virtual int PotionGBID { get; set; }
	}
}

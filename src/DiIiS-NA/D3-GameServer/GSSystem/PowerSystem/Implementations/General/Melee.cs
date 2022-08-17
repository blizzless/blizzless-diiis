//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(30592)]  // Weapon_Melee_Instant.pow
	public class WeaponMeleeInstant : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			WeaponDamage(GetBestMeleeEnemy(), 1.00f, DamageType.Physical);
			yield break;
		}

		public override float GetActionSpeed()
		{
			// for some reason the formula for _Instant.pow does not multiply by 1.1 even though it should
			// manually scale melee speed
			return base.GetActionSpeed() * 1.1f;
		}
	}

	[ImplementsPowerSNO(136189)]  // Diablo_ClawRip.pow
	public class Diablo_ClawRip : ActionTimedSkill
	{
		public override IEnumerable<TickTimer> Main()
		{
			WeaponDamage(GetBestMeleeEnemy(13f), 1.00f, DamageType.Physical);
			yield break;
		}

		public override float GetActionSpeed()
		{
			return base.GetActionSpeed() * 1.2f;
		}
	}
}

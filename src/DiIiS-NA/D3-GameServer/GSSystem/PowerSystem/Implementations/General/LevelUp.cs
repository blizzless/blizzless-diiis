using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(85954)] //g_levelup.pow
	public class LevelUpBlast : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			User.PlayEffectGroup(Player.LevelUpEffects[User.Attributes[GameAttribute.Level] - 1]);
			yield return WaitSeconds(0.6f);
			WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(2)), ScriptFormula(0), DamageType.Physical);
			yield break;
		}
	}

	[ImplementsPowerSNO(252038)] //g_levelup_aa.pow
	public class LevelUpParagonBlast : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			User.PlayEffectGroup(252023);
			yield return WaitSeconds(0.6f);
			WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(2)), ScriptFormula(0), DamageType.Physical);
			yield break;
		}
	}
}

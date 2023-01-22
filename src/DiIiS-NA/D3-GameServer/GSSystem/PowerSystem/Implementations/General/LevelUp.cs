//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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

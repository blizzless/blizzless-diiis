using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		ActorSno._treasuregoblin_a, //treasureGoblin_A
		ActorSno._treasuregoblin_b, //treasureGoblin_B
		ActorSno._treasuregoblin_c //treasureGoblin_C
	)]
	public class Goblin : Monster
	{
		public Goblin(World world, ActorSno sno, TagMap tags)//, int level = 1)
			: base(world, sno, tags)
		{
			// Override minimap icon in markerset tags
			WalkSpeed = 0;
			Brain = new MonsterBrain(this);
			Attributes[GameAttributes.MinimapActive] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 123152;
			Attributes[GameAttributes.Hitpoints_Max] *= 3f;
			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 0f;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 0f;
			//(this.Brain as MonsterBrain).AddPresetPower(54055); //TreasureGoblinPause
			(Brain as MonsterBrain).AddPresetPower(105371); //TreasureGoblin_Escape
		}

		public override bool Reveal(Player player)
		{
			if (World.SNO == WorldSno.a1dun_spidercave_01 || World.SNO == WorldSno.trout_oldtistram_cellar_3)
			{
				Destroy();
				return false;
			}

			return base.Reveal(player);
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Unique;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}
	}
}

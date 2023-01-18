//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using System;

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
			this.WalkSpeed = 0;
			this.Brain = new MonsterBrain(this);
			this.Attributes[GameAttribute.MinimapActive] = true;
			//this.Attributes[GameAttribute.MinimapIconOverride] = 123152;
			this.Attributes[GameAttribute.Hitpoints_Max] *= 3f;
			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			//this.Attributes[GameAttribute.Immune_To_Charm] = true;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 0f;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 0f;
			//(this.Brain as MonsterBrain).AddPresetPower(54055); //TreasureGoblinPause
			(this.Brain as MonsterBrain).AddPresetPower(105371); //TreasureGoblin_Escape
		}

		public override bool Reveal(Player player)
		{
			if (this.World.SNO == WorldSno.a1dun_spidercave_01 || this.World.SNO == WorldSno.trout_oldtistram_cellar_3)
			{
				this.Destroy();
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

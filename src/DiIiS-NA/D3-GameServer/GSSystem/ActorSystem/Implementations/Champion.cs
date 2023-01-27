using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public class Champion : Monster
	{

		private int NamePrefix = -1;
		private int NameSuffix = -1;

		public Champion(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.Hitpoints_Max] *= 4.0f;
			Attributes[GameAttribute.Immune_To_Charm] = true;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] *= 2.5f;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] *= 2.5f;
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
			//MonsterAffixGenerator.Generate(this, this.World.Game.Difficulty + 1);
			Attributes[GameAttribute.Movement_Scalar] = Attributes[GameAttribute.Movement_Scalar] * 0.5f;
			Attributes[GameAttribute.Run_Speed_Granted] = Attributes[GameAttribute.Run_Speed_Granted] * 0.5f;
			Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			WalkSpeed = 0.3f;
			NamePrefix = MonsterAffixGenerator.GeneratePrefixName();
			NameSuffix = MonsterAffixGenerator.GenerateSuffixName();

		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player)) return false;

			var affixGbids = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
			for (int i = 0; i < AffixList.Count - 1; i++)
			{
				affixGbids[i] = AffixList[i].AffixGbid;
			}
			
			player.InGameClient.SendMessage(new RareMonsterNamesMessage()
			{
				ann = DynamicID(player),
				RareNames = new int[2] { NamePrefix, NameSuffix },
				MonsterAffixes = affixGbids
			});
			
			return true;
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Champion;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}
	}
}

using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public class RareMinion : Monster
	{
		public RareMinion(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.Hitpoints_Max] *= 3.0f;
			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max];
			Attributes[GameAttributes.Damage_Weapon_Min, 0] *= 1.5f;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] *= 1.5f;

			//MonsterAffixGenerator.Generate(this, this.World.Game.Difficulty + 1);
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
				RareNames = new int[2] { -1, -1 },
				MonsterAffixes = affixGbids
			});

			return true;
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Minion;
			}
			set
			{
				// TODO MonsterQuality setter not implemented. Throwing a NotImplementedError is catched as message not beeing implemented and nothing works anymore...
			}
		}
	}
}

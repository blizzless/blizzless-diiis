using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.MessageSystem;
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._tyrael)]
	class Tyrael : InteractiveNPC, IUpdateable
	{
		public Tyrael(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Brain = new AggressiveNPCBrain(this); // erekose			 

			// lookup GameBalance MonsterLevels.gam asset
			var monsterLevels = (GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			var monsterData = (Monster.Target as MonsterFF);

			Attributes[GameAttributes.Level] = 1;
			Attributes[GameAttributes.Hitpoints_Max] = 100000;
			Attributes[GameAttributes.Hitpoints_Cur] = Attributes[GameAttributes.Hitpoints_Max_Total];
			Attributes[GameAttributes.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f;
			Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 5f;
			WalkSpeed = 0.3f * monsterData.AttributeModifiers[129];  // TODO: this is probably multiplied by something erekose the 0.3 is because he is way too fast otherwise
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList) && World.Game.CurrentQuest == 87700)
			{
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));
			}

			base.ReadTags();
		}

		public void Update(int tickCounter)
		{
			if (Brain == null)
				return;

			Brain.Update(tickCounter);
		}
	}
}

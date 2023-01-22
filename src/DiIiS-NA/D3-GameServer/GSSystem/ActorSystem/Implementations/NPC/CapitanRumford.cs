//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		ActorSno._captainrumfoord,
		ActorSno._angel_trooper_a,
		ActorSno._bastionskeepguard_melee_a_01_stationedguard,
		ActorSno._caldeumguard_cleaver_a,
		ActorSno._caldeumguard_cleaver_a_jarulf,
		ActorSno._caldeumguard_cleaver_a_town,
		ActorSno._caldeumguard_spear_imperial_town,
		ActorSno._x1_westmhub_guardnohelmunarmed,
		ActorSno._x1_westmhub_guard_patrol,
		ActorSno._x1_westmhub_guard,
		ActorSno._x1_westmhub_guardnohelm,
		ActorSno._x1_malthael,
		ActorSno._x1_imperius
	)]
	class CaptainRumford : InteractiveNPC, IUpdateable
	{
		public CaptainRumford(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Brain = new AggressiveNPCBrain(this); // erekose			 

			// lookup GameBalance MonsterLevels.gam asset
			var monsterLevels = (GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			var monsterData = (Monster.Target as MonsterFF);

			Attributes[GameAttribute.Level] = 1;
			Attributes[GameAttribute.Hitpoints_Max] = 100000;
			Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
			Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
			Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
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

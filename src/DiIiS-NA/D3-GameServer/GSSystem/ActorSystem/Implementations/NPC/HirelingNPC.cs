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
	class HirelingNPC : InteractiveNPC, IUpdateable
	{
		public HirelingNPC(MapSystem.World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Brain = new AggressiveNPCBrain(this); // erekose			 

			// lookup GameBalance MonsterLevels.gam asset
			var monsterLevels = (GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
			var monsterData = (Monster.Target as MonsterFF);

			this.Attributes[GameAttribute.Level] = 1;
			this.Attributes[GameAttribute.Hitpoints_Max] = 100000;
			this.Attributes[GameAttribute.Hitpoints_Cur] = this.Attributes[GameAttribute.Hitpoints_Max_Total];
			this.Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
			this.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
			this.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
			this.WalkSpeed = 0.3f * monsterData.AttributeModifiers[129];  // TODO: this is probably multiplied by something erekose the 0.3 is because he is way too fast otherwise
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList) && this.World.Game.CurrentQuest == 87700)
			{
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));
			}

			base.ReadTags();
		}

		public void Update(int tickCounter)
		{
			if (this.Brain == null)
				return;

			this.Brain.Update(tickCounter);
		}
	}
}

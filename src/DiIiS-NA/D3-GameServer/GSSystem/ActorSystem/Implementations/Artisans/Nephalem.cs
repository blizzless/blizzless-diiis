//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(ActorSno._p1_lr_tieredrift_nephalem /* P1_LR_TieredRift_Nephalem.acr */)]
	public class Nephalem : Artisan
	{
		public Nephalem(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.NPC_Is_Operatable] = true;
			Attributes[GameAttribute.Is_NPC] = true;
			Attributes[GameAttribute.In_Tiered_Loot_Run_Level] = 0;
			Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.NPC_Has_Interact_Options, 0] = true;
			Attributes[GameAttribute.NPC_Has_Interact_Options, 1] = true;
			Attributes[GameAttribute.NPC_Has_Interact_Options, 2] = true;
			Attributes[GameAttribute.NPC_Has_Interact_Options, 3] = true;
			//this.Attributes[GameAttribute.Conversation_Icon] = 2;
			//this.ForceConversationSNO = 
		}

		public override void OnCraft(Player player)
		{
			if (World.Game.ActiveNephalemKilledBoss == true)
			{
				World.Game.ActiveNephalemKilledBoss = false;
				foreach (var plr in World.Game.Players.Values)
				{
					plr.InGameClient.SendMessage(new QuestCounterMessage()
					{
						snoQuest = 0x00052654,
						snoLevelArea = 0x000466E2,
						StepID = 34,
						TaskIndex = 0,
						Checked = 1,
						Counter = 1
					});
					plr.InGameClient.SendMessage(new QuestUpdateMessage()
					{
						snoQuest = 0x00052654,
						snoLevelArea = 0x000466E2,
						StepID = 46,
						DisplayButton = true,
						Failed = false
					});

				}
			}

			player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = DynamicID(player) });
			player.ArtisanInteraction = "Mystic";
		}

		public override bool Reveal(Player player)
		{

			return base.Reveal(player);
		}
	}
}

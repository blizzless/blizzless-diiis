using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
	[HandledSNO(ActorSno._p1_lr_tieredrift_nephalem /* P1_LR_TieredRift_Nephalem.acr */)]
	public class Nephalem : Artisan
	{
		public Nephalem(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.In_Tiered_Loot_Run_Level] = 0;
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

			base.OnCraft(player);
			player.CurrentArtisan = ArtisanType.Nephalem;
		}

		public override bool Reveal(Player player)
		{

			return base.Reveal(player);
		}
	}
}

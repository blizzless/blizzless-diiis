//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(
		ActorSno._a1_id_all_book_of_cain,
		ActorSno._a2_id_all_book_of_cain,
		ActorSno._a3_id_all_book_of_cain,
		ActorSno._a5_id_all_book_of_cain_b
	)]
	class CainBook : Gizmo
	{
		public CainBook(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.TeamID] = 1;
			this.Attributes[GameAttribute.MinimapActive] = true;
		}

		public override bool Reveal(Player player)
		{
			player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Map.MapMarkerInfoMessage()
			{
				HashedName = DiIiS_NA.Core.Helpers.Hash.StringHashHelper.HashItemName("CainBook"),
				Place = new WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
				ImageInfo = 300665,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = -1,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = true,
				PlayerUseFlags = 0
			});

			return base.Reveal(player);
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			int idDuration = 60;
			player.StartCasting(idDuration, new Action(() => {
				foreach (var itm in player.Inventory.GetBackPackItems().Where(i => i.Unidentified))
					itm.Identify();
			}));

			World.BroadcastIfRevealed(plr => new PlayAnimationMessage
			{
				ActorID = this.DynamicID(plr),
				AnimReason = 5,
				UnitAniimStartTime = 0,
				tAnim = new PlayAnimationMessageSpec[]
				{
					new PlayAnimationMessageSpec()
					{
						Duration = idDuration,
						AnimationSNO = AnimationSet.TagMapAnimDefault[AnimationSetKeys.Opening],
						PermutationIndex = 0,
						AnimationTag = 0,
						Speed = 1
					}
				}

			}, this);

			base.OnTargeted(player, message);
		}
	}
}

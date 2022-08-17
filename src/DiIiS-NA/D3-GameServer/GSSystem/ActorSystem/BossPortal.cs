//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
////Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	public class BossPortal : Actor
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		private int Encounter;
		private int DestArea;
		private int DestWorld;
		private int DestPoint;
		private ResolvedPortalDestination Destination { get; set; }

		public BossPortal(World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.Field2 = 0x9;//16;

			this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.Untargetable] = false;
			switch (((this.ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[MarkerKeys.BossEncounter].Target as DiIiS_NA.Core.MPQ.FileFormats.BossEncounter).Worlds[0])
			{
				case 60713:
					DestArea = 60714; break;
				case 73261:
					DestArea = 19789; break;
				case 109143:
					DestArea = 109149; break;
				case 182976:
					DestArea = 62726; this.Scale = 0.75f; break;
				case 159580:
					DestArea = 58494; break;
				case 58493:
					DestArea = 58494; break;
				case 174449:
					DestArea = 130163; break;
				case 103910:
					DestArea = 119882; break;
				case 214956:
					DestArea = 215396; break;
				case 166640:
					DestArea = 143648; break;
				case 195200:
					DestArea = 195268; break;
				case 78839:
					DestArea = 90881; break;
				case 109561:
					DestArea = 109563; break;
				case 328484:
					DestArea = 330576; break;
				case 380753:
					DestArea = 378681; break;
				case 226713:
					DestArea = 112580; break;
				case 121214:
					DestArea = 111516; break;
				case 186552:
					DestArea = 201250; break;
				case 119650:
					DestArea = 139274; break;
				case 297771:
					DestArea = 287220; break;
				case 295225:
					DestArea = 295228; break;


			}
			DestWorld = ((this.ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[MarkerKeys.BossEncounter].Target as DiIiS_NA.Core.MPQ.FileFormats.BossEncounter).Worlds[0];
			DestPoint = ((this.ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[MarkerKeys.BossEncounter].Target as DiIiS_NA.Core.MPQ.FileFormats.BossEncounter).I11;
			//get EncounterSNO
			switch (this.ActorSNO.Id)
			{
				case 168932: //CainIntro
					this.Encounter = 168925;
					break;
				case 159573: //Leoric
					this.Encounter = 159592;
					break;
				case 183032: //SpiderQueen
					this.Encounter = 181436;
					break;
				case 158944: //Butcher
					this.Encounter = 158915;
					break;
				case 195234: //Maghda
					this.Encounter = 195226;
					break;
				case 159578: //Cain Death
					this.Encounter = 159591;
					break;
				//case 159578: //Belial Audience
				//this.Encounter = 162231;
				//break;
				case 159580: //Adria Rescue
					this.Encounter = 159584;
					break;
				case 159581: //Zoltun Kulle
					this.Encounter = 159586;
					break;
				case 159574: //Belial
					this.Encounter = 159585;
					break;
				case 226784: //SiegeBreaker
					this.Encounter = 226716;
					break;
				case 161278: //Cydaea
					this.Encounter = 161246;
					break;
				case 159575: //Azmodan
					this.Encounter = 159582;
					break;
				case 159576: //Adria_Betrayal
					this.Encounter = 159583;
					break;
				case 182963: //Iskatu
					this.Encounter = 182960;
				    break;
				case 161276: //Rakanoth
					this.Encounter = 161247;
					break;
				case 220551: //Imperius_Spire
					this.Encounter = 220541;
					break;
				case 161279: //Diablo
					this.Encounter = 161280;
					break;
				case 309883: //Urzael
					this.Encounter = 298128;
					break;
				case 293005: //Adria
					this.Encounter = 293007;
					break;
				case 296314: //BatteringRam
					this.Encounter = 296315;
					break;
				case 374257: //Malthael
					this.Encounter = 278965;
					break;
				case 380766:
					this.Encounter = 380760;
					break;
				default:
					this.Encounter = 0;
					break;
			}

			this.Destination = new DiIiS_NA.GameServer.MessageSystem.Message.Fields.ResolvedPortalDestination
			{
				WorldSNO = DestWorld,
				DestLevelAreaSNO = DestArea,
				StartingPointActorTag = DestPoint
			};
		}
		public static bool setActorOperable(World world, Int32 snoId, bool status)
		{
			var actor = world.GetActorBySNO(snoId);

			if (actor == null)
				return false;

			actor.Attributes[GameAttribute.Team_Override] = (status ? -1 : 2);
			actor.Attributes[GameAttribute.Untargetable] = !status;
			actor.Attributes[GameAttribute.NPC_Is_Operatable] = status;
			actor.Attributes[GameAttribute.Operatable] = status;
			actor.Attributes[GameAttribute.Operatable_Story_Gizmo] = status;
			actor.Attributes[GameAttribute.Disabled] = !status;
			actor.Attributes[GameAttribute.Immunity] = !status;
			actor.Attributes.BroadcastChangedIfRevealed();
			return true;
		}
		public override bool Reveal(Player player)
		{
			if (this.ActorSNO.Name.EndsWith("EventPortal")) return false;
			if (!base.Reveal(player))
				return false;
			/*
			player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Portal.PortalSpecifierMessage()
			{
				ActorID = this.DynamicID(player),
				Destination = new ResolvedPortalDestination()
				{
					WorldSNO = DestWorld,
					StartingPointActorTag = DestArea,
					DestLevelAreaSNO = DestPoint
				}
			});
			//*/
			player.InGameClient.SendMessage(new DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal.PortalSpecifierMessage()
			{
				ActorID = this.DynamicID(player),
				Destination = this.Destination
			});
			return true;
		}

		public override void OnPlayerApproaching(Player player)
		{
			
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Trace("(OnTargeted) BossPortal has been activated, Id: {0}", this.ActorSNO.Id);
			if (this.Encounter == 0) return;
			//if (this.World.Game.CurrentEncounter.activated) return;

			this.World.Game.CurrentEncounter.activated = true;
			this.World.Game.CurrentEncounter.SnoId = this.Encounter;

			foreach (Player plr in this.World.Game.Players.Values)
				plr.InGameClient.SendMessage(new BossEncounterMessage(Opcodes.BossJoinEncounterMessage)
				{
					PlayerIndex = plr.PlayerIndex,
					snoEncounter = this.Encounter
				});
		}
	}
}

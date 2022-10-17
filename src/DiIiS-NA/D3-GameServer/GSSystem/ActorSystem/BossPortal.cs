//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
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

		// "EventPortal" actors
		private static readonly ActorSno[] eventPortals = new ActorSno[]
		{
			ActorSno._x1_westmhub_scoundreleventportal,
			ActorSno._x1_westmhub_templareventportal,
			ActorSno._x1_westmhub_enchantresseventportal,
			ActorSno._x1_westmhub_jewelereventportal,
		};

		public override ActorType ActorType { get { return ActorType.Gizmo; } }

		private int Encounter;
		private int DestArea;
		private int DestWorld;
		private int DestPoint;
		private ResolvedPortalDestination Destination { get; set; }

		public BossPortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Field2 = 0x9;//16;

			this.Attributes[GameAttribute.MinimapActive] = true;
			this.Attributes[GameAttribute.Untargetable] = false;
            var bossEncounter = ((this.ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[MarkerKeys.BossEncounter].Target as DiIiS_NA.Core.MPQ.FileFormats.BossEncounter);
			DestWorld = bossEncounter.Worlds[0];
			switch (DestWorld)
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
            DestPoint = bossEncounter.I11;
			//get EncounterSNO
			switch (this.SNO)
			{
				case ActorSno._boss_portal_cainintro: //CainIntro
					this.Encounter = 168925;
					break;
				case ActorSno._boss_portal_skeletonking: //Leoric
					this.Encounter = 159592;
					break;
				case ActorSno._boss_portal_spiderqueen: //SpiderQueen
					this.Encounter = 181436;
					break;
				case ActorSno._boss_portal_butcher: //Butcher
					this.Encounter = 158915;
					break;
				case ActorSno._boss_portal_maghda: //Maghda
					this.Encounter = 195226;
					break;
				case ActorSno._boss_portal_binkleshulkout: //Cain Death
					this.Encounter = 159591;
					break;
				//case 159578: //Belial Audience
				//this.Encounter = 162231;
				//break;
				case ActorSno._boss_portal_adriasewer: //Adria Rescue
					this.Encounter = 159584;
					break;
				case ActorSno._boss_portal_blacksoulstone: //Zoltun Kulle
					this.Encounter = 159586;
					break;
				case ActorSno._boss_portal_belial: //Belial
					this.Encounter = 159585;
					break;
				case ActorSno._boss_portal_siegebreaker: //SiegeBreaker
					this.Encounter = 226716;
					break;
				case ActorSno._boss_portal_mistressofpain: //Cydaea
					this.Encounter = 161246;
					break;
				case ActorSno._boss_portal_azmodan: //Azmodan
					this.Encounter = 159582;
					break;
				case ActorSno._boss_portal_adriabetrayal: //Adria_Betrayal
					this.Encounter = 159583;
					break;
				case ActorSno._boss_portal_1000monsterfight: //Iskatu
					this.Encounter = 182960;
				    break;
				case ActorSno._boss_portal_despair: //Rakanoth
					this.Encounter = 161247;
					break;
				case ActorSno._bossportal_imperius_spirebase: //Imperius_Spire
					this.Encounter = 220541;
					break;
				case ActorSno._boss_portal_diablo: //Diablo
					this.Encounter = 161280;
					break;
				case ActorSno._x1_urzael_bossportal: //Urzael
					this.Encounter = 298128;
					break;
				case ActorSno._x1_boss_portal_adria: //Adria
					this.Encounter = 293007;
					break;
				case ActorSno._x1_boss_portal_batteringram: //BatteringRam
					this.Encounter = 296315;
					break;
				case ActorSno._x1_fortress_malthael_boss_portal: //Malthael
					this.Encounter = 278965;
					break;
				case ActorSno._boss_portal_greed:
					this.Encounter = 380760;
					break;
				default:
					this.Encounter = 0;
					break;
			}

			this.Destination = new ResolvedPortalDestination
            {
				WorldSNO = DestWorld,
				DestLevelAreaSNO = DestArea,
				StartingPointActorTag = DestPoint
			};
		}
		public static bool SetActorOperable(World world, ActorSno sno, bool status)
		{
			var actor = world.GetActorBySNO(sno);

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
			if (eventPortals.Contains(this.SNO)) return false;
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
			player.InGameClient.SendMessage(new PortalSpecifierMessage()
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
			Logger.Trace("(OnTargeted) BossPortal has been activated, Id: {0}", this.SNO);
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

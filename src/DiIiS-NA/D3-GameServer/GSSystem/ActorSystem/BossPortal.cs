using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Portal;

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
			Field2 = 0x9;//16;

			Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.Untargetable] = false;
            var bossEncounter = ((ActorSNO.Target as DiIiS_NA.Core.MPQ.FileFormats.Actor).TagMap[MarkerKeys.BossEncounter].Target as DiIiS_NA.Core.MPQ.FileFormats.BossEncounter);
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
					DestArea = 62726; Scale = 0.75f; break;
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
			switch (SNO)
			{
				case ActorSno._boss_portal_cainintro: //CainIntro
					Encounter = 168925;
					break;
				case ActorSno._boss_portal_skeletonking: //Leoric
					Encounter = 159592;
					break;
				case ActorSno._boss_portal_spiderqueen: //SpiderQueen
					Encounter = 181436;
					break;
				case ActorSno._boss_portal_butcher: //Butcher
					Encounter = 158915;
					break;
				case ActorSno._boss_portal_maghda: //Maghda
					Encounter = 195226;
					break;
				case ActorSno._boss_portal_binkleshulkout: //Cain Death
					Encounter = 159591;
					break;
				//case 159578: //Belial Audience
				//this.Encounter = 162231;
				//break;
				case ActorSno._boss_portal_adriasewer: //Adria Rescue
					Encounter = 159584;
					break;
				case ActorSno._boss_portal_blacksoulstone: //Zoltun Kulle
					Encounter = 159586;
					break;
				case ActorSno._boss_portal_belial: //Belial
					Encounter = 159585;
					break;
				case ActorSno._boss_portal_siegebreaker: //SiegeBreaker
					Encounter = 226716;
					break;
				case ActorSno._boss_portal_mistressofpain: //Cydaea
					Encounter = 161246;
					break;
				case ActorSno._boss_portal_azmodan: //Azmodan
					Encounter = 159582;
					break;
				case ActorSno._boss_portal_adriabetrayal: //Adria_Betrayal
					Encounter = 159583;
					break;
				case ActorSno._boss_portal_1000monsterfight: //Iskatu
					Encounter = 182960;
				    break;
				case ActorSno._boss_portal_despair: //Rakanoth
					Encounter = 161247;
					break;
				case ActorSno._bossportal_imperius_spirebase: //Imperius_Spire
					Encounter = 220541;
					break;
				case ActorSno._boss_portal_diablo: //Diablo
					Encounter = 161280;
					break;
				case ActorSno._x1_urzael_bossportal: //Urzael
					Encounter = 298128;
					break;
				case ActorSno._x1_boss_portal_adria: //Adria
					Encounter = 293007;
					break;
				case ActorSno._x1_boss_portal_batteringram: //BatteringRam
					Encounter = 296315;
					break;
				case ActorSno._x1_fortress_malthael_boss_portal: //Malthael
					Encounter = 278965;
					break;
				case ActorSno._boss_portal_greed:
					Encounter = 380760;
					break;
				default:
					Encounter = 0;
					break;
			}

			Destination = new ResolvedPortalDestination
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
			if (eventPortals.Contains(SNO)) return false;
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
				ActorID = DynamicID(player),
				Destination = Destination
			});
			return true;
		}

		public override void OnPlayerApproaching(Player player)
		{
			
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			Logger.Trace("(OnTargeted) BossPortal has been activated, Id: {0}", SNO);
			if (Encounter == 0) return;
			//if (this.World.Game.CurrentEncounter.activated) return;

			World.Game.CurrentEncounter.activated = true;
			World.Game.CurrentEncounter.SnoId = Encounter;

			foreach (Player plr in World.Game.Players.Values)
				plr.InGameClient.SendMessage(new BossEncounterMessage(Opcodes.BossJoinEncounterMessage)
				{
					PlayerIndex = plr.PlayerIndex,
					snoEncounter = Encounter
				});
		}
	}
}

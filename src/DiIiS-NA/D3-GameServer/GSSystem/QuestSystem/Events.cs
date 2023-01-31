using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem
{
	public class Events : QuestRegistry
	{
		static readonly Logger Logger = LogManager.CreateLogger();

		public Events(Game game) : base(game)
		{
		}

		public override void SetQuests()
		{
			#region Raising Recruits (followers)
			Game.QuestManager.SideQuests.Add(154195, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[154195].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[154195].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				OnAdvance = new Action(() => { //free Guards
					ListenKill(ActorSno._terrordemon_a_unique_1000monster, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[154195].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //guard winches
					ListenKill(ActorSno._terrordemon_a_unique_1000monster, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[154195].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion

			#region A1Farmer - Clear Farm
			Game.QuestManager.SideQuests.Add(81925, new Quest { RewardXp = 500, RewardGold = 250, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			//var quest_data = (Mooege.Common.MPQ.FileFormats.Quest)Mooege.Common.MPQ.MPQStorage.Data.Assets[Common.Types.SNO.SNOGroup.Actor][81925].Data;
			//1,7,4
			Game.QuestManager.SideQuests[81925].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});
			Game.QuestManager.SideQuests[81925].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 7,
				OnAdvance = new Action(() => {
					ListenKill(ActorSno._fleshpitflyerspawner_b_event_farmambush, 4, new SideAdvance());
				})
			});
			Game.QuestManager.SideQuests[81925].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				OnAdvance = new Action(() => {
					var world = Game.GetWorld(WorldSno.trout_town);
					StartConversation(world, 60182);
					if (!Game.Players.IsEmpty)
						world.SpawnMonster(ActorSno._fleshpitflyer_b_farmhouseambush_unique, Game.Players.First().Value.Position);
					ListenKill(ActorSno._fleshpitflyer_b_farmhouseambush_unique, 1, new SideAdvance());
				})
			});
			Game.QuestManager.SideQuests[81925].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => {
					StartConversation(Game.GetWorld(WorldSno.trout_town), 60184);
				})
			});
			#endregion

			#region Last Stand of Ancients
			Game.QuestManager.SideQuests.Add(121745, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			//102008 tomb
			GlobalListenInteract(ActorSno._temp_story_trigger_enabled, 1, new StartSideQuest(121745, true));

			Game.QuestManager.SideQuests[121745].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[121745].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //defend yourself
					var world = Game.GetWorld(WorldSno.trout_town);
					script = new Invasion(world.Players.First().Value.Position, 50f, new List<ActorSno> { ActorSno._skeleton_b, ActorSno._skeletonarcher_b }, 30f, ActorSno._shield_skeleton_nephchamp, false);
					script.Execute(world);
					ListenKill(ActorSno._shield_skeleton_nephchamp, 3, new SideAdvance()); //mob skeleton
				})
			});

			Game.QuestManager.SideQuests[121745].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region The Crumbling Vault
			Game.QuestManager.SideQuests.Add(120396, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(19794, new StartSideQuest(120396, true));

			Game.QuestManager.SideQuests[120396].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[120396].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //escape to treasure room
					SetQuestTimer(120396, 180f, Game.GetWorld(WorldSno.a2dun_zolt_timed01_level01), new SideAbandon());
					ListenTeleport(168200, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[120396].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players)
						if (Game.QuestManager.QuestTimerEstimate >= 90f)
							plr.Value.GrantAchievement(74987243307689);
				})
			});
			#endregion
			#region Revenge of Gharbad
			Game.QuestManager.SideQuests.Add(225253, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenConversation(81069, new StartSideQuest(225253, true));

			Game.QuestManager.SideQuests[225253].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[225253].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //Break Totems
					ListenKill(ActorSno._trout_highlands_goatman_totem_gharbad, 2, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[225253].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 12,
				OnAdvance = new Action(() => { //kill shamans
                    var world = Game.GetWorld(WorldSno.trout_town);
					script = new Invasion(
						world.GetActorBySNO(ActorSno._trout_highlands_chiefgoatmenmummyrack_a_gharbadevent).Position,
						30f,
						new List<ActorSno> { ActorSno._goatman_ranged_b_event_gharbad_the_weak, ActorSno._goatman_melee_b_event_gharbad_the_weak },
						15f,
						ActorSno._goatman_shaman_a_event_gharbad_the_weak,
						false
					);
					script.Execute(world);
					ListenKill(ActorSno._goatman_shaman_a_event_gharbad_the_weak, 1, new SideAdvance()); //mob shaman
				})
			});

			Game.QuestManager.SideQuests[225253].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 14,
				OnAdvance = new Action(() => { //talk to Gharbad
					ListenConversation(81099, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[225253].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //Kill gharbad
					(Game.GetWorld(WorldSno.trout_town).GetActorBySNO(ActorSno._gharbad_the_weak_ghost) as Gharbad).Resurrect();
					ListenKill(ActorSno._goatmutant_melee_a_unique_gharbad, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[225253].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete

				})
			});
			#endregion
			#region Rygnar's Idol
			Game.QuestManager.SideQuests.Add(30857, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(19795, new StartSideQuest(30857, true));

			Game.QuestManager.SideQuests[30857].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 0,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[30857].Steps.Add(0, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Poltahr
					ListenInteract(ActorSno._a2c2poltahr, 1, new LaunchConversation(18039));
					ListenConversation(18039, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[30857].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 17,
				OnAdvance = new Action(() => { //find Idol
					AddFollower(Game.GetWorld(WorldSno.a2c2dun_zolt_treasurehunter), ActorSno._a2c2poltahr);
					ListenProximity(ActorSno._interactlocation, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[30857].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 19,
				OnAdvance = new Action(() => { //get idol
					StartConversation(Game.GetWorld(WorldSno.a2c2dun_zolt_treasurehunter), 18038);
					ListenInteract(ActorSno._a2dun_zolt_pedestal, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[30857].Steps.Add(19, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //stop ambush
					foreach (var spawner in Game.GetWorld(WorldSno.a2c2dun_zolt_treasurehunter).GetActorsBySNO(ActorSno._spawner_skeletonmage_cold_a))
					{
						spawner.World.SpawnMonster(ActorSno._skeletonmage_cold_a, spawner.Position);
					}
					ListenKill(ActorSno._skeletonmage_cold_a, 4, new LaunchConversation(18037));
					ListenConversation(18037, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[30857].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					DestroyFollower(ActorSno._a2c2poltahr);
				})
			});
			#endregion
			#region Lost Treasure of Khan Dakab
			Game.QuestManager.SideQuests.Add(158596, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(158594, new StartSideQuest(158596, true));

			Game.QuestManager.SideQuests[158596].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 16,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[158596].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 18,
				OnAdvance = new Action(() => { //find lever
					var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large);
					SetActorOperable(world, ActorSno._a2dun_aqd_godhead_door_largepuzzle, false);
					var spots = world.GetActorsBySNO(ActorSno._boxtrigger__one_shot_);
					world.SpawnMonster(ActorSno._a2dun_aqd_act_lever_facepuzzle_01, spots.PickRandom().Position);
					ListenInteract(ActorSno._a2dun_aqd_act_lever_facepuzzle_01, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158596].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 20,
				OnAdvance = new Action(() => { //enter vault
					SetActorOperable(Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large), ActorSno._a2dun_aqd_godhead_door_largepuzzle, true);
					ListenInteract(ActorSno._a2dun_aqd_godhead_door_largepuzzle, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158596].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 22,
				OnAdvance = new Action(() => { //claim treasure
					ListenInteract(ActorSno._a2dun_aqd_chest_special_facepuzzle_large, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158596].Steps.Add(22, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //kill unique
                    var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_large);
                    foreach (var spawner in world.GetActorsBySNO(ActorSno._spawner_ghost_d_facepuzzle))
					{
						spawner.World.SpawnMonster(ActorSno._ghost_d, spawner.Position);
					}
                    world.SpawnMonster(ActorSno._ghost_d_facepuzzleunique, world.GetActorBySNO(ActorSno._spawner_ghost_d_facepuzzleunique).Position);
                    ListenKill(ActorSno._ghost_d_facepuzzleunique, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158596].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region Sardar's Treasure
			Game.QuestManager.SideQuests.Add(158377, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(158384, new StartSideQuest(158377, true));

			Game.QuestManager.SideQuests[158377].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[158377].Steps.Add(13, new QuestStep
            {
				Completed = false,
				Saveable = false,
				NextStep = 17,
				OnAdvance = new Action(() => { //find lever
                    var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_small);
                    SetActorOperable(world, ActorSno._a2dun_aqd_godhead_door, false);
					var spots = world.GetActorsBySNO(ActorSno._boxtrigger__one_shot_);
                    world.SpawnMonster(ActorSno._a2dun_aqd_act_lever_facepuzzle_01, spots.PickRandom().Position);
                    ListenInteract(ActorSno._a2dun_aqd_act_lever_facepuzzle_01, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158377].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 22,
				OnAdvance = new Action(() => { //enter vault
					SetActorOperable(Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_small), ActorSno._a2dun_aqd_godhead_door, true);
					ListenInteract(ActorSno._a2dun_aqd_godhead_door, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158377].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 19,
				OnAdvance = new Action(() => { //claim treasure
					ListenInteract(ActorSno._a2dun_aqd_chest_rare_facepuzzlesmall, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158377].Steps.Add(19, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //kill unique
					var world = Game.GetWorld(WorldSno.a2dun_aqd_oasis_randomfacepuzzle_small);
					foreach (var spawner in world.GetActorsBySNO(ActorSno._spawner_fastmummy_climb_a_smallfacepuzzle))
					{
						spawner.World.SpawnMonster(ActorSno._fastmummy_a, spawner.Position);
					}
					world.SpawnMonster(ActorSno._fastmummy_b_facepuzzleunique, world.GetActorBySNO(ActorSno._spawner_fastmummy_b_smallfacepuzzleunique).Position);
					ListenKill(ActorSno._fastmummy_b_facepuzzleunique, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[158377].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region x1_Event_WaveFight_ArmyOfTheDead
			Game.QuestManager.SideQuests.Add(365751, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[365751].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[365751].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365751].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(Game.SideQuestGizmo.Position, 30f, new List<ActorSno> { ActorSno._x1_zombieskinny_a }, ActorSno._x1_zombieskinny_skeleton_a_lr_boss);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._x1_zombieskinny_skeleton_a_lr_boss, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365751].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedShrine)
						(Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_WaveFight_BloodClanAssault
			Game.QuestManager.SideQuests.Add(368092, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[368092].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[368092].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[368092].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(
						Game.SideQuestGizmo.Position,
						30f,
						new List<ActorSno> { ActorSno._goatman_shaman_b, ActorSno._goatman_melee_a, ActorSno._goatman_melee_b, ActorSno._goatman_melee_c, ActorSno._goatman_ranged_a },
						ActorSno._goatman_shaman_a_event_gharbad_the_weak
					);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._goatman_shaman_a_event_gharbad_the_weak, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[368092].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedShrine)
						(Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_WaveFight_CultistLegion
			Game.QuestManager.SideQuests.Add(365033, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[365033].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[365033].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365033].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(Game.SideQuestGizmo.Position, 30f, new List<ActorSno> { ActorSno._triunecultist_c_event }, ActorSno._cultist_crownleader);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._cultist_crownleader, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365033].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedShrine)
						(Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_GhoulSwarm
			Game.QuestManager.SideQuests.Add(365305, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[365305].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[365305].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365305].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(Game.SideQuestGizmo.Position, 30f, new List<ActorSno> { ActorSno._ghoul_a, ActorSno._ghoul_b }, ActorSno._ghoul_b_speedkill_rare);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._ghoul_b_speedkill_rare, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[365305].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedChest)
						(Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_ArmyOfHell
			Game.QuestManager.SideQuests.Add(368306, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[368306].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[368306].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[368306].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(Game.SideQuestGizmo.Position, 30f, new List<ActorSno> { ActorSno._demontrooper_a_catapult }, ActorSno._x1_demontrooper_chronodemon_test_a);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._x1_demontrooper_chronodemon_test_a, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[368306].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedChest)
						(Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_Bonepit
			Game.QuestManager.SideQuests.Add(369332, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[369332].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[369332].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => { //find lever
					ListenInteract(Game.SideQuestGizmo.SNO, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[369332].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(
						Game.SideQuestGizmo.Position,
						30f,
						new List<ActorSno> { ActorSno._skeletonking_shield_skeleton, ActorSno._skeletonking_skeleton, ActorSno._skeletonarcher_jail },
						ActorSno._skeleton_necrojar
					);
					script.Execute(Game.SideQuestGizmo.World);
					ListenKill(ActorSno._skeleton_necrojar, 1, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[369332].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					if (Game.SideQuestGizmo != null && Game.SideQuestGizmo is CursedChest)
						(Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A1
			Game.QuestManager.SideQuests.Add(356988, new Quest { RewardXp = 10000, RewardGold = 1000, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			Game.QuestManager.SideQuests[356988].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});
			Game.QuestManager.SideQuests[356988].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					var TristHab = Game.GetWorld(WorldSno.x1_tristram_adventure_mode_hub);
					var Tyrael = TristHab.GetActorBySNO(ActorSno._tyrael_heaven) as ActorSystem.InteractiveNPC;
					if (Tyrael != null)
					{
						Tyrael.ForceConversationSNO = 352539;
					}
					//114622
					//ListenInteract(114622, 1, new LaunchConversation(352539));
					ListenConversation(352539, new SideAdvance());
				})
			});
			Game.QuestManager.SideQuests[356988].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA1");
						cache.Attributes[GameAttribute.Act] = 0;
						cache.Attributes[GameAttribute.Item_Quality_Level] = Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if(plr.Toon.IsSeasoned) plr.GrantCriteria(74987258781748);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A2
			Game.QuestManager.SideQuests.Add(356994, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[356994].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[356994].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(ActorSno._tyrael_heaven, 1, new LaunchConversation(357038));
					ListenConversation(357038, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[356994].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA2");
						cache.Attributes[GameAttribute.Act] = 100;
						cache.Attributes[GameAttribute.Item_Quality_Level] = Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.IsSeasoned) plr.GrantCriteria(74987247833299);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A3
			Game.QuestManager.SideQuests.Add(356996, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[356996].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[356996].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(ActorSno._tyrael_heaven, 1, new LaunchConversation(357040));
					ListenConversation(357040, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[356996].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA3");
						cache.Attributes[GameAttribute.Act] = 200;
						cache.Attributes[GameAttribute.Item_Quality_Level] = Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);

						if (plr.Toon.IsSeasoned) plr.GrantCriteria(74987248811185);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A4
			Game.QuestManager.SideQuests.Add(356999, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[356999].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[356999].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(ActorSno._tyrael_heaven, 1, new LaunchConversation(357021));
					ListenConversation(357021, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[356999].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA4");
						cache.Attributes[GameAttribute.Act] = 300;
						cache.Attributes[GameAttribute.Item_Quality_Level] = Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.IsSeasoned) plr.GrantCriteria(74987256262166);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A5
			Game.QuestManager.SideQuests.Add(357001, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			Game.QuestManager.SideQuests[357001].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				OnAdvance = new Action(() => {
				})
			});

			Game.QuestManager.SideQuests[357001].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(ActorSno._tyrael_heaven, 1, new LaunchConversation(357042));
					ListenConversation(357042, new SideAdvance());
				})
			});

			Game.QuestManager.SideQuests[357001].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				OnAdvance = new Action(() => { //complete
					foreach (var plr in Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA5");
						cache.Attributes[GameAttribute.Act] = 400;
						cache.Attributes[GameAttribute.Item_Quality_Level] = Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.IsSeasoned) plr.GrantCriteria(74987249495955);
					}
				})
			});
			#endregion
		}
	}
}

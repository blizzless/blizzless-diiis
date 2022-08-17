//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

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
			this.Game.QuestManager.SideQuests.Add(154195, new Quest { RewardXp = 0, RewardGold = 0, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[154195].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 6,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[154195].Steps.Add(6, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //free Guards
					ListenKill(196102, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[154195].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //guard winches
					ListenKill(196102, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[154195].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion

			#region A1Farmer - Clear Farm
			this.Game.QuestManager.SideQuests.Add(81925, new Quest { RewardXp = 500, RewardGold = 250, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			//var quest_data = (Mooege.Common.MPQ.FileFormats.Quest)Mooege.Common.MPQ.MPQStorage.Data.Assets[Common.Types.SNO.SNOGroup.Actor][81925].Data;
			//1,7,4
			this.Game.QuestManager.SideQuests[81925].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});
			this.Game.QuestManager.SideQuests[81925].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 7,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					ListenKill(81982, 4, new SideAdvance());
				})
			});
			this.Game.QuestManager.SideQuests[81925].Steps.Add(7, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					StartConversation(this.Game.GetWorld(71150), 60182);
					if (this.Game.Players.Count > 0)
						this.Game.GetWorld(71150).SpawnMonster(260231, this.Game.Players.First().Value.Position);
					ListenKill(260231, 1, new SideAdvance());
				})
			});
			this.Game.QuestManager.SideQuests[81925].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
					StartConversation(this.Game.GetWorld(71150), 60184);
				})
			});
			#endregion

			#region Last Stand of Ancients
			this.Game.QuestManager.SideQuests.Add(121745, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			//102008 tomb
			GlobalListenInteract(102008, 1, new StartSideQuest(121745, true));

			this.Game.QuestManager.SideQuests[121745].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[121745].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //defend yourself
					script = new Invasion(this.Game.GetWorld(71150).Players.First().Value.Position, 50f, new List<int> { 5395, 5347 }, 30f, 112134, false);
					script.Execute(this.Game.GetWorld(71150));
					ListenKill(112134, 3, new SideAdvance()); //mob skeleton
				})
			});

			this.Game.QuestManager.SideQuests[121745].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region The Crumbling Vault
			this.Game.QuestManager.SideQuests.Add(120396, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(19794, new StartSideQuest(120396, true));

			this.Game.QuestManager.SideQuests[120396].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[120396].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //escape to treasure room
					SetQuestTimer(120396, 180f, this.Game.GetWorld(50596), new SideAbandon());
					ListenTeleport(168200, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[120396].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players)
						if (this.Game.QuestManager.QuestTimerEstimate >= 90f)
							plr.Value.GrantAchievement(74987243307689);
				})
			});
			#endregion
			#region Revenge of Gharbad
			this.Game.QuestManager.SideQuests.Add(225253, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenConversation(81069, new StartSideQuest(225253, true));

			this.Game.QuestManager.SideQuests[225253].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 4,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[225253].Steps.Add(4, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //Break Totems
					ListenKill(225252, 2, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[225253].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 12,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill shamans
					script = new Invasion(this.Game.GetWorld(71150).GetActorBySNO(223597).Position, 30f, new List<int> { 81618, 81090 }, 15f, 81093, false);
					script.Execute(this.Game.GetWorld(71150));
					ListenKill(81093, 1, new SideAdvance()); //mob shaman
				})
			});

			this.Game.QuestManager.SideQuests[225253].Steps.Add(12, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 14,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk to Gharbad
					ListenConversation(81099, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[225253].Steps.Add(14, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //Kill gharbad
					(this.Game.GetWorld(71150).GetActorBySNO(81068) as Gharbad).Resurrect();
					ListenKill(81342, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[225253].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete

				})
			});
			#endregion
			#region Rygnar's Idol
			this.Game.QuestManager.SideQuests.Add(30857, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(19795, new StartSideQuest(30857, true));

			this.Game.QuestManager.SideQuests[30857].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 0,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[30857].Steps.Add(0, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Poltahr
					ListenInteract(2935, 1, new LaunchConversation(18039));
					ListenConversation(18039, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[30857].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 17,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find Idol
					AddFollower(this.Game.GetWorld(2812), 2935);
					ListenProximity(4522, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[30857].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 19,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //get idol
					StartConversation(this.Game.GetWorld(2812), 18038);
					ListenInteract(307, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[30857].Steps.Add(19, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //stop ambush
					foreach (var spawner in this.Game.GetWorld(2812).GetActorsBySNO(54571))
					{
						spawner.World.SpawnMonster(5367, spawner.Position);
					}
					ListenKill(5367, 4, new LaunchConversation(18037));
					ListenConversation(18037, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[30857].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					DestroyFollower(2935);
				})
			});
			#endregion
			#region Lost Treasure of Khan Dakab
			this.Game.QuestManager.SideQuests.Add(158596, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(158594, new StartSideQuest(158596, true));

			this.Game.QuestManager.SideQuests[158596].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 16,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[158596].Steps.Add(16, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 18,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					setActorOperable(this.Game.GetWorld(158593), 207615, false);
					var spots = this.Game.GetWorld(158593).GetActorsBySNO(3461);
					this.Game.GetWorld(158593).SpawnMonster(219879, spots[FastRandom.Instance.Next(spots.Count())].Position);
					ListenInteract(219879, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158596].Steps.Add(18, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 20,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					setActorOperable(this.Game.GetWorld(158593), 207615, true);
					ListenInteract(207615, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158596].Steps.Add(20, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 22,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //claim treasure
					ListenInteract(190524, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158596].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill unique
					foreach (var spawner in this.Game.GetWorld(158593).GetActorsBySNO(219919))
					{
						spawner.World.SpawnMonster(4198, spawner.Position);
					}
					this.Game.GetWorld(158593).SpawnMonster(207605, this.Game.GetWorld(158593).GetActorBySNO(219918).Position);
					ListenKill(207605, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158596].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region Sardar's Treasure
			this.Game.QuestManager.SideQuests.Add(158377, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			GlobalListenTeleport(158384, new StartSideQuest(158377, true));

			this.Game.QuestManager.SideQuests[158377].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[158377].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 17,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					setActorOperable(this.Game.GetWorld(157882), 153836, false);
					var spots = this.Game.GetWorld(157882).GetActorsBySNO(3461);
					this.Game.GetWorld(157882).SpawnMonster(219879, spots[FastRandom.Instance.Next(spots.Count())].Position);
					ListenInteract(219879, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158377].Steps.Add(17, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 22,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					setActorOperable(this.Game.GetWorld(157882), 153836, true);
					ListenInteract(153836, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158377].Steps.Add(22, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 19,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //claim treasure
					ListenInteract(190708, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158377].Steps.Add(19, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //kill unique
					foreach (var spawner in this.Game.GetWorld(157882).GetActorsBySNO(219901))
					{
						spawner.World.SpawnMonster(4104, spawner.Position);
					}
					this.Game.GetWorld(157882).SpawnMonster(203795, this.Game.GetWorld(157882).GetActorBySNO(219885).Position);
					ListenKill(203795, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[158377].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
				})
			});
			#endregion
			#region x1_Event_WaveFight_ArmyOfTheDead
			this.Game.QuestManager.SideQuests.Add(365751, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[365751].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[365751].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365751].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 276465 }, 375188);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(375188, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365751].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedShrine)
						(this.Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_WaveFight_BloodClanAssault
			this.Game.QuestManager.SideQuests.Add(368092, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[368092].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[368092].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[368092].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 375, 4282, 4283, 4284, 4286 }, 81093);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(81093, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[368092].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedShrine)
						(this.Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_WaveFight_CultistLegion
			this.Game.QuestManager.SideQuests.Add(365033, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[365033].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 13,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[365033].Steps.Add(13, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365033].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 90960 }, 104043);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(104043, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365033].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedShrine)
						(this.Game.SideQuestGizmo as CursedShrine).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_GhoulSwarm
			this.Game.QuestManager.SideQuests.Add(365305, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[365305].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[365305].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365305].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 4201, 4202 }, 371013);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(371013, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[365305].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedChest)
						(this.Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_ArmyOfHell
			this.Game.QuestManager.SideQuests.Add(368306, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[368306].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[368306].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[368306].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 220474 }, 301232);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(301232, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[368306].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedChest)
						(this.Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_Event_Horde_Bonepit
			this.Game.QuestManager.SideQuests.Add(369332, new Quest { RewardXp = 100, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[369332].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 11,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[369332].Steps.Add(11, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //find lever
					ListenInteract(this.Game.SideQuestGizmo.ActorSNO.Id, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[369332].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 2,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //enter vault
					script = new WavedInvasion(this.Game.SideQuestGizmo.Position, 30f, new List<int> { 51339, 51340, 230834 }, 239339);
					script.Execute(this.Game.SideQuestGizmo.World);
					ListenKill(239339, 1, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[369332].Steps.Add(2, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					if (this.Game.SideQuestGizmo != null && this.Game.SideQuestGizmo is CursedChest)
						(this.Game.SideQuestGizmo as CursedChest).Activate();
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A1
			this.Game.QuestManager.SideQuests.Add(356988, new Quest { RewardXp = 10000, RewardGold = 1000, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });
			this.Game.QuestManager.SideQuests[356988].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});
			this.Game.QuestManager.SideQuests[356988].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					var TristHab = this.Game.GetWorld(332336);
					var Tyrael = TristHab.GetActorBySNO(114622) as ActorSystem.InteractiveNPC;
					if (Tyrael != null)
					{
						Tyrael.ForceConversationSNO = 352539;
					}
					//114622
					//ListenInteract(114622, 1, new LaunchConversation(352539));
					ListenConversation(352539, new SideAdvance());
				})
			});
			this.Game.QuestManager.SideQuests[356988].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA1");
						cache.Attributes[GameAttribute.Act] = 0;
						cache.Attributes[GameAttribute.Item_Quality_Level] = this.Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if(plr.Toon.isSeassoned) plr.GrantCriteria(74987258781748);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A2
			this.Game.QuestManager.SideQuests.Add(356994, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[356994].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[356994].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(114622, 1, new LaunchConversation(357038));
					ListenConversation(357038, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[356994].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA2");
						cache.Attributes[GameAttribute.Act] = 100;
						cache.Attributes[GameAttribute.Item_Quality_Level] = this.Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.isSeassoned) plr.GrantCriteria(74987247833299);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A3
			this.Game.QuestManager.SideQuests.Add(356996, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[356996].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[356996].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(114622, 1, new LaunchConversation(357040));
					ListenConversation(357040, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[356996].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA3");
						cache.Attributes[GameAttribute.Act] = 200;
						cache.Attributes[GameAttribute.Item_Quality_Level] = this.Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);

						if (plr.Toon.isSeassoned) plr.GrantCriteria(74987248811185);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A4
			this.Game.QuestManager.SideQuests.Add(356999, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[356999].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[356999].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(114622, 1, new LaunchConversation(357021));
					ListenConversation(357021, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[356999].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA4");
						cache.Attributes[GameAttribute.Act] = 300;
						cache.Attributes[GameAttribute.Item_Quality_Level] = this.Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.isSeassoned) plr.GrantCriteria(74987256262166);
					}
				})
			});
			#endregion
			#region x1_AdventureMode_BountyTurnin_A5
			this.Game.QuestManager.SideQuests.Add(357001, new Quest { RewardXp = 10000, RewardGold = 100, Completed = false, Saveable = false, NextQuest = -1, Steps = new Dictionary<int, QuestStep> { } });

			this.Game.QuestManager.SideQuests[357001].Steps.Add(-1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => {
				})
			});

			this.Game.QuestManager.SideQuests[357001].Steps.Add(1, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = 3,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //talk with Tyrael
					ListenInteract(114622, 1, new LaunchConversation(357042));
					ListenConversation(357042, new SideAdvance());
				})
			});

			this.Game.QuestManager.SideQuests[357001].Steps.Add(3, new QuestStep
			{
				Completed = false,
				Saveable = false,
				NextStep = -1,
				Objectives = new List<Objective> { new Objective { Limit = 1, Counter = 0 } },
				OnAdvance = new Action(() => { //complete
					foreach (var plr in this.Game.Players.Values)
					{
						var cache = ItemGenerator.Cook(plr, "HoradricCacheA5");
						cache.Attributes[GameAttribute.Act] = 400;
						cache.Attributes[GameAttribute.Item_Quality_Level] = this.Game.Difficulty;
						cache.Attributes[GameAttribute.IsCrafted] = true;
						plr.Inventory.PickUp(cache);
						if (plr.Toon.isSeassoned) plr.GrantCriteria(74987249495955);
					}
				})
			});
			#endregion
		}
	}
}

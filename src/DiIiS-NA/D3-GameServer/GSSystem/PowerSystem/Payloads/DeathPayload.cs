//Blizzless Project 2022
using System;
//Blizzless Project 2022
using System.Collections.Generic;
//Blizzless Project 2022
using System.Linq;
//Blizzless Project 2022
using System.Threading.Tasks;
//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat;
//Blizzless Project 2022
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022
using DiIiS_NA.LoginServer.Toons;
//Blizzless Project 2022
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.GeneratorsSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ItemsSystem;
//Blizzless Project 2022
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Text;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public class DeathPayload : Payload
	{
		static readonly Logger Logger = LogManager.CreateLogger();
		public DamageType DeathDamageType;
		public bool LootAndExp; //HACK: As we currently just give out random exp and loot, this is in to prevent giving this out for mobs that shouldn't give it.

		public bool Successful = false;
		public bool AutomaticHitEffects = true;

		public DeathPayload(PowerContext context, DamageType deathDamageType, Actor target, bool grantsLootAndExp = true)
			: base(context, target)
		{
			LootAndExp = grantsLootAndExp;
			DeathDamageType = deathDamageType;

			if (Target == null)
			{
				if (target == null)
					return;
				else
					Target = target;
			}
			if (Target.World == null) return;
			if (!Target.Visible) return;
			if (!Target.World.Game.Working) return;
			if (Target.Dead)
				return;

			if (Target is Player plr)
			{
				if(plr.World.Game.NephalemGreater)
					plr.Attributes[GameAttribute.Tiered_Loot_Run_Death_Count]++;
				if (plr.SkillSet.HasPassive(218501) && plr.World.BuffManager.GetFirstBuff<SpiritVesselCooldownBuff>(plr) == null) //SpiritWessel (wd)
				{
					plr.Attributes[GameAttribute.Hitpoints_Cur] = plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.15f;
					plr.Attributes.BroadcastChangedIfRevealed();
					plr.World.BuffManager.AddBuff(plr, plr, new ActorGhostedBuff());
					plr.World.BuffManager.AddBuff(plr, plr, new SpiritVesselCooldownBuff());
					return;
				}
				if (plr.SkillSet.HasPassive(156484) && plr.World.BuffManager.GetFirstBuff<NearDeathExperienceCooldownBuff>(plr) == null) //NearDeathExperience (monk)
				{
					plr.Attributes[GameAttribute.Hitpoints_Cur] = plr.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.35f;
					plr.Attributes[GameAttribute.Resource_Cur, 3] = plr.Attributes[GameAttribute.Resource_Max_Total, 3] * 0.35f;
					plr.Attributes.BroadcastChangedIfRevealed();
					plr.World.BuffManager.AddBuff(plr, plr, new NearDeathExperienceCooldownBuff());
					return;
				}
			}
			if (Target is Hireling hireling)
			{
				hireling.Dead = true;

				if (hireling.Dead)
				{
					hireling.Attributes[GameAttribute.Hitpoints_Cur] = hireling.Attributes[GameAttribute.Hitpoints_Max_Total];
					hireling.Attributes.BroadcastChangedIfRevealed();
					hireling.Dead = false;
				}

				return;
			}
			Successful = true;
		}

		public void Apply()
		{
			var PositionOfDeath = Target.Position;
			if (!Target.World.Game.Working) return;

			if (Target.Attributes.Contains(GameAttribute.Quest_Monster))
			{
				Target.Attributes[GameAttribute.Quest_Monster] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}

			if (new System.Diagnostics.StackTrace().FrameCount > 35) // some arbitrary limit
			{
				Logger.Error("StackOverflowException prevented!: {0}", System.Environment.StackTrace);
				return;
			}

			if (Target is NecromancerSkeleton_A { Master: Player masterPlr } skeletonA)
			{
				//(this.Target as NecromancerSkeleton_A).Master+
				masterPlr.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Pet.PetDetachMessage()
				{
					PetId = skeletonA.DynamicID((skeletonA.Master as Player))
				});
				masterPlr.NecroSkeletons.Remove(skeletonA);
			}
			if (Target is Minion { Master: Player masterPlr2 } and (BaseGolem or IceGolem or BoneGolem or DecayGolem or ConsumeFleshGolem or BloodGolem))
            {
				masterPlr2.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Pet.PetDetachMessage()
				{
					PetId = Target.DynamicID(((Target as Minion).Master as Player))
				});
				masterPlr2.ActiveGolem = null;
			}
			if (Target is Player user)
			{
				if (user.SkillSet.HasPassive(208779)) //Grenadier (DH)
				{
					user.World.PowerManager.RunPower(user, 208779);
				}

				user.Attributes.BroadcastChangedIfRevealed();

				DoPlayerDeath();
				return;
			}

			if (Context != null)
				if (Context.User is Player player)  //Hitpoints_On_Kill
					if (player.Attributes[GameAttribute.Hitpoints_On_Kill] > 0)
						player.AddHP(player.Attributes[GameAttribute.Hitpoints_On_Kill]);

			// HACK: add to hackish list thats used to defer deleting actor and filter it from powers targetting
			if (!(Target is Boss))
				Target.World.PowerManager.AddDeletingActor(Target);

			if (Target is Living actor)
			{
				if (actor.Brain != null)
					actor.Brain.Kill();
			}

			if (Target is Monster target)
			{
				if (target.Brain != null)
					target.Brain.Kill();

				target.World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
				{
					ActorID = target.DynamicID(plr),
					CollFlags = 0
				}, target);

			}

			if (Target is Minion minionTarget)
			{
				if (minionTarget.Master != null && minionTarget.Master is Player)
				{
					(minionTarget.Master as Player).Followers.Remove(minionTarget.GlobalID);
					(minionTarget.Master as Player).FreeFollowerIndex(minionTarget.SNO);
				}
				if (minionTarget.Brain != null)
					minionTarget.Brain.Kill();

				LootAndExp = false;
			}

			if (Target is Champion)
			{
				bool championsAlive = Target.GetActorsInRange<Champion>(1000).Where(c => c.GroupId == Target.GroupId && c.Attributes[GameAttribute.Hitpoints_Cur] > 0).ToList().Count > 0;
				if (championsAlive)
					LootAndExp = false;
			}

			// send this death payload to buffs
			Target.World.BuffManager.SendTargetPayload(Target, this);

			if (Context != null)
				if (Context.User != null)
					Target.World.BuffManager.SendTargetPayload(Context.User, this);

			Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage()
			{
				ActorId = Target.DynamicID(plr),
				Effect = Effect.Unknown12,
			}, Target);

			Target.World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.PlayIdleAnimationMessage)
			{
				ActorID = Target.DynamicID(plr)
			}, Target);

			// special death animation
			switch (Target.SNO)
			{
				//Boss-A1 Q2
				case ActorSno._skeleton_a_cain_unique:
				//Йондар
				case ActorSno._adventurer_d_templarintrounique:
				//Темные жрецы
				case ActorSno._triunevessel_event31:
				//Падшие
				case ActorSno._fallengrunt_a:
                    Target.PlayAnimation(11, AnimationSno.triunesummoner_death_02_persistentblood, 1f);
                    break;
				//Разнощик чумы
				case ActorSno._fleshpitflyer_b:
				//Пчелы
				case ActorSno._sandwasp_a:
				case ActorSno._fleshpitflyer_leoric_inferno:
					Target.PlayAnimation(11, AnimationSno.fleshpitflyer_death, 1f);
					break;
				//X1_LR_Boss_Angel_Corrupt_A
				case ActorSno._x1_lr_boss_angel_corrupt_a:
                    Target.PlayAnimation(11, AnimationSno.angel_corrupt_death_01, 1f);
                    break;
				default:
					var animation = FindBestDeathAnimationSNO();
					if (animation != AnimationSno._NONE)
						Target.PlayAnimation(11, animation, 1f);
					else
					{
						Logger.Warn("Death animation not found: ActorSNOId = {0}", Target.SNO);
					}
					break;
			}

			Target.World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.CancelACDTargetMessage)
			{
				ActorID = Target.DynamicID(plr),
			}, Target);


			// remove all buffs and running powers before deleting actor
			Target.World.BuffManager.RemoveAllBuffs(Target);
			Target.World.PowerManager.CancelAllPowers(Target);

			Target.Attributes[GameAttribute.Deleted_On_Server] = true;
			Target.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
			Target.Attributes.BroadcastChangedIfRevealed();

			Target.World.BroadcastIfRevealed(plr => new DeathFadeTimeMessage()
			{
				Field0 = Target.DynamicID(plr),
				Field1 = 300,
				Field2 = 200,
				Field3 = true
			}, Target);

			if (Context?.User != null)
				if (Math.Abs(Context.User.Attributes[GameAttribute.Item_Power_Passive, 247640] - 1) < 0.001 ||
					Math.Abs(Context.User.Attributes[GameAttribute.Item_Power_Passive, 249963] - 1) < 0.001 ||
					Math.Abs(Context.User.Attributes[GameAttribute.Item_Power_Passive, 249954] - 1) < 0.001 ||
					(float)FastRandom.Instance.NextDouble() < 0.1f ||
					Target.World.SNO == WorldSno.a1dun_random_level01)
					switch ((int)DeathDamageType.HitEffect)
					{
						case 0: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.Gore }, Target); break;
						case 1: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.GoreFire }, Target); break;
						case 2: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.GoreElectro }, Target); break;
						case 3: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.IceBreak }, Target); break;
						case 4: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.GorePoison }, Target); break;
						case 5: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.GoreArcane }, Target); break;
						case 6: Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = Target.DynamicID(plr), Effect = Effect.GoreHoly }, Target); break;
					}

			if (Context != null)
				if (Context.User is Player player && Math.Abs(player.Attributes[GameAttribute.Level] - Target.Attributes[GameAttribute.Level]) < 5)
					player.KilledSeasonalTempCount++;

			if (Context?.User is Player plr2)
				if (Context.World.BuffManager.HasBuff<LandOfDead.ZBuff>(Context.User))
				{
					plr2.BuffStreakKill += 1;
					if (plr2.BuffStreakKill == 10 || plr2.BuffStreakKill == 20)
					{
						//(this.Context.User as Player).Attributes[_Buff_Icon_End_TickN, PowerSNO]
					}
				}


			if (Target.SNO == ActorSno._a4dun_garden_corruption_monster) //Gardens of Hope
			{
				//Garden of Hope
				if (Target.World.SNO == WorldSno.a4dun_garden_of_hope_01)
				{
					//Check if there are portals
					var PortalToHell = Target.World.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal); //{[Actor] [Type: Gizmo] SNOId:224890 DynamicId: 280 Position: x:696,681 y:695,4387 z:0,2636871 Name: a4_Heaven_Gardens_HellPortal}
					if (PortalToHell.Count == 0)
					{
						var Corruptions = Target.World.GetActorsBySNO(ActorSno._a4dun_garden_corruption_monster);
						if (Corruptions.Count > 1)
						{
							if (RandomHelper.Next(0, 30) > 26)
							{
								Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
								HellPortal.EnterWorld(Target.Position);
								Context.User.World.SpawnMonster(ActorSno._diablo_vo, Context.User.Position);
								StartConversation(Target.World, 217226);
							}
						}
						else
						{
							Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
							HellPortal.EnterWorld(Target.Position);
							Context.User.World.SpawnMonster(ActorSno._diablo_vo, Context.User.Position);
							StartConversation(Target.World, 217226);
						}
					}
				}
				//Second floor of the gardens of hope
				else if (Target.World.SNO == WorldSno.a4dun_garden_of_hope_random)
				{ //Check if there are portals
					var PortalToHell = Target.World.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal); //{[Actor] [Type: Gizmo] SNOId:224890 DynamicId: 280 Position: x:696,681 y:695,4387 z:0,2636871 Name: a4_Heaven_Gardens_HellPortal}
					if (PortalToHell.Count == 0)
					{
						var Corruptions = Target.World.GetActorsBySNO(ActorSno._a4dun_garden_corruption_monster);
						if (Corruptions.Count > 1)
						{
							if (RandomHelper.Next(0, 30) > 26)
							{
								Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
								HellPortal.EnterWorld(Target.Position);
								if (Context.User.World.GetActorsBySNO(ActorSno._diablo_vo).Count == 0)
									Context.User.World.SpawnMonster(ActorSno._diablo_vo, Context.User.Position);
								StartConversation(Target.World, 217228);
							}
						}
						else
						{
							Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
							HellPortal.EnterWorld(Target.Position);
							if (Context.User.World.GetActorsBySNO(ActorSno._diablo_vo).Count == 0)
								Context.User.World.SpawnMonster(ActorSno._diablo_vo, Context.User.Position);
							StartConversation(Target.World, 217228);
						}
					}
				}
			}

			// Spawn Random item and give exp for each player in range
			List<Player> players = Target.GetPlayersInRange(100f);
			foreach (Player plr in players)
			{
				int grantedExp = 0;
				if (plr.Attributes[GameAttribute.Level] <= Target.Attributes[GameAttribute.Level])
					grantedExp = (int)(Player.LevelBorders[plr.Attributes[GameAttribute.Level]] / (40 * Target.Attributes[GameAttribute.Level] * 0.85f) * (Target is Monster ? Math.Min((Target as Monster).HpMultiplier, 3f) : 1f));
				else
					grantedExp = (int)(Player.LevelBorders[plr.Attributes[GameAttribute.Level]] / (40 * Target.Attributes[GameAttribute.Level] * 0.85f) * (1 - Math.Abs(plr.Attributes[GameAttribute.Level] - Target.Attributes[GameAttribute.Level]) / 20));

				grantedExp = (int)(grantedExp * (plr.Attributes[GameAttribute.Experience_Bonus_Percent] + 1));
				grantedExp += (int)plr.Attributes[GameAttribute.Experience_Bonus];



				if (LootAndExp)
				{
					grantedExp = (int)(grantedExp * plr.World.Game.XpModifier);

					float tempEXP = grantedExp * Config.Instance.RateExp;

					plr.UpdateExp(Math.Max((int)tempEXP, 1));
					var a = (int)plr.Attributes[GameAttribute.Experience_Bonus];
					var a1 = (int)plr.Attributes[GameAttribute.Experience_Bonus_Percent];

					plr.KilledMonstersTempCount++;
				}

				if (Target is Champion or Rare or Boss or Unique)
				{
					plr.KilledElitesTempCount++;
				}

				//achievements here
				if (Target is Monster monster)
				{
					if (plr.Toon.Class == ToonClass.DemonHunter)
					{
						if (monster.MonsterType == (int)DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Demon)
							plr.AddAchievementCounter(74987243307065, 1);

						if (PowerMath.Distance2D(plr.Position, monster.Position) >= 45f)
							plr.AddAchievementCounter(74987243307061, 1);

						if (monster.Attributes[GameAttribute.Feared])
							plr.AddAchievementCounter(74987243307064, 1);

						if (Context.PowerSNO == 75301)
						{
							plr.SpikeTrapsKilled++;
							if (plr.SpikeTrapsKilled >= 15)
								plr.GrantAchievement(74987243307060);

							plr.AddTimedAction(5f, new Action<int>((q) => plr.SpikeTrapsKilled--));
						}
					}
					if (plr.Toon.Class == ToonClass.Monk)
					{
						if (plr.Attributes[GameAttribute.Resource_Cur, 3] < plr.Attributes[GameAttribute.Resource_Max_Total, 3])
							plr.AddAchievementCounter(74987243307550, 1);
					}
					if (plr.Toon.Class == ToonClass.Wizard)
					{
						if (monster.Attributes[GameAttribute.Frozen])
							plr.AddAchievementCounter(74987243307585, 1);
					}
					if (plr.Toon.Class == ToonClass.WitchDoctor)
					{
						if (Context.User.Attributes[GameAttribute.Team_Override] == 1)
							plr.AddAchievementCounter(74987243307564, 1);
					}

					switch (monster)
					{
						case Champion:
							plr.CheckKillMonsterCriteria(monster.SNO, 1);
							break;
						case Rare:
							plr.CheckKillMonsterCriteria(monster.SNO, 2);
							break;
						case Unique:
							plr.CheckKillMonsterCriteria(monster.SNO, 4);
							break;
					}
				}

				if (Target is Unique)
				{
					if (LoreRegistry.Lore.ContainsKey(Target.World.SNO) && LoreRegistry.Lore[Target.World.SNO].chests_lore.ContainsKey(Target.SNO))
						foreach (int loreId in LoreRegistry.Lore[Target.World.SNO].chests_lore[Target.SNO])
							if (!plr.HasLore(loreId))
							{
								Target.World.DropItem(Target, null, ItemGenerator.CreateLore(plr, loreId));
								break;
							}
				}

				if (plr.SkillSet.HasPassive(218191) && PowerMath.Distance2D(plr.Position, Target.Position) <= 20f + plr.Attributes[GameAttribute.Gold_PickUp_Radius]) //GraveInjustice (WD)
				{
					plr.AddHP(plr.Attributes[GameAttribute.Hitpoints_Max_Total] / 100f);
					plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_Max_Total, 0] / 100f);
					foreach (var cdBuff in plr.World.BuffManager.GetBuffs<CooldownBuff>(plr))
						cdBuff.Reduce(60);
				}

				if (plr.SkillSet.HasPassive(357218) && PowerMath.Distance2D(plr.Position, Target.Position) <= 15f) //Fervor (Crusader)
				{
					plr.World.BuffManager.AddBuff(plr, plr, new FervorBuff());
				}

				if (Target.World.BuffManager.HasBuff<Leech.Rune_E_Buff>(Target))
					foreach (var a in Target.World.BuffManager.GetBuffs<CooldownBuff>(Context.User))
						if (a.TargetPowerSNO == 30211)
							a.Reduce(60);

				if (Target.World.BuffManager.HasBuff<Leech.Rune_C_Buff>(Target))
				{
					Context.User.AddHP(Context.User.Attributes[GameAttribute.Hitpoints_On_Kill_Total] * 2f);
				}


				if (Target.World.BuffManager.HasBuff<CrusaderJudgment.JudgedDebuffRooted>(Target))    //Crusader -> Judgment -> Conversion
					if (Target.World.BuffManager.GetFirstBuff<CrusaderJudgment.JudgedDebuffRooted>(Target).conversion)
						if (FastRandom.Instance.Next() < 0.2f)
						{
							var avatar = new AvatarMelee(plr.World, Context, 0, 1f, Context.WaitSeconds(7f));
							avatar.Brain.DeActivate();
							avatar.Position = PowerContext.RandomDirection(plr.Position, 3f, 8f);
							avatar.Attributes[GameAttribute.Untargetable] = true;

							avatar.EnterWorld(avatar.Position);

                            Task.Delay(1000).ContinueWith(d =>
							{
								(avatar as Minion).Brain.Activate();
								avatar.Attributes[GameAttribute.Untargetable] = false;
								avatar.Attributes.BroadcastChangedIfRevealed();
							});
						}

				if (plr.SkillSet.HasPassive(208571) && PowerMath.Distance2D(plr.Position, Target.Position) <= 12f + plr.Attributes[GameAttribute.Gold_PickUp_Radius] && FastRandom.Instance.Next(100) < 5) //CircleOfLife (WD)
				{
					var dog = new ZombieDog(plr.World, plr, 0);
					dog.Brain.DeActivate();
					dog.Position = PowerContext.RandomDirection(plr.Position, 3f, 8f);
					dog.Attributes[GameAttribute.Untargetable] = true;
					dog.EnterWorld(dog.Position);
					dog.PlayActionAnimation(AnimationSno.zombiedog_summon_01);
					Context.DogsSummoned++;

                    Task.Delay(1000).ContinueWith(d =>
					{
						dog.Brain.Activate();
						dog.Attributes[GameAttribute.Untargetable] = false;
						dog.Attributes.BroadcastChangedIfRevealed();
						dog.PlayActionAnimation(AnimationSno.zombiedog_idle_01);
					});
				}

				if (plr.SkillSet.HasPassive(341344)) //Dominance (Wiz)
				{
					plr.World.BuffManager.AddBuff(plr, plr, new DominanceBuff());
				}

				if (plr.SkillSet.HasPassive(296572)) //Rampage (Burb)
				{
					plr.World.BuffManager.AddBuff(plr, plr, new RampageBuff());
				}
				if (Context != null)
					if (Context.DogsSummoned >= 3)
						plr.GrantAchievement(74987243307567);
			}
			Logger.Trace("Killed monster, id: {0}, level {1}", Target.SNO, Target.Attributes[GameAttribute.Level]);


			//handling quest triggers
			if (Target.World.Game.QuestProgress.QuestTriggers.ContainsKey((int)Target.SNO))
			{
				var trigger = Target.World.Game.QuestProgress.QuestTriggers[(int)Target.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					Target.World.Game.QuestProgress.UpdateCounter((int)Target.SNO);
					if (trigger.count == Target.World.Game.QuestProgress.QuestTriggers[(int)Target.SNO].counter)
						trigger.questEvent.Execute(Target.World); // launch a questEvent
				}
				else
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
				{
					Target.World.Game.QuestProgress.UpdateCounter((int)Target.SNO);
				}
			}
			else if (Target.World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)Target.SNO))
			{
				var trigger = Target.World.Game.SideQuestProgress.QuestTriggers[(int)Target.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					Target.World.Game.SideQuestProgress.UpdateSideCounter((int)Target.SNO);
					if (trigger.count == Target.World.Game.SideQuestProgress.QuestTriggers[(int)Target.SNO].counter)
						trigger.questEvent.Execute(Target.World); // launch a questEvent
				}
			}
			if (Target.World == null)
				return;
			foreach (var bounty in Target.World.Game.QuestManager.Bounties)
			{	if (Target.OriginalLevelArea == -1)
					Target.OriginalLevelArea = Target.CurrentScene.Specification.SNOLevelAreas[0];
				bounty.CheckKill((int)Target.SNO, Target.OriginalLevelArea, Target.World.SNO);
			}

			//Nephalem Rift
			if ((Target.CurrentScene.Specification.SNOLevelAreas[0] == 332339 || Target.CurrentScene.Specification.SNOLevelAreas[0] == 288482) && Target.World.Game.ActiveNephalemTimer && Target.World.Game.ActiveNephalemKilledMobs == false)
			{
				Target.World.Game.ActiveNephalemProgress += (1f * (Target.Quality + 1));
				Player Master = null;
				foreach (var plr in Target.World.Game.Players.Values)
				{
					if (plr.PlayerIndex == 0)
						Master = plr;
					plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.KillCounterRefresh)
					{

					});

					plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
					{
						Field0 = Target.World.Game.ActiveNephalemProgress
					});



					if (Target.World.Game.ActiveNephalemProgress > 650)
					{
						Target.World.Game.ActiveNephalemKilledMobs = true;
						if (Target.World.Game.NephalemGreater)
						{
							plr.InGameClient.SendMessage(new QuestCounterMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = 13,
								TaskIndex = 0,
								Checked = 1,
								Counter = 1
							});
							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = 16,
								DisplayButton = true,
								Failed = false
							});
						}
						else
						{
							plr.InGameClient.SendMessage(new QuestCounterMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = 1,
								TaskIndex = 0,
								Checked = 1,
								Counter = 1
							});
							plr.InGameClient.SendMessage(new QuestUpdateMessage()
							{
								snoQuest = 0x00052654,
								snoLevelArea = 0x000466E2,
								StepID = 3,
								DisplayButton = true,
								Failed = false
							});
						}
						plr.InGameClient.SendMessage(new PlayMusicMessage(Opcodes.PlayMusicMessage)
						{
							SNO = 0x0005BBD8
						});

						plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameChatTextMessage) { Message = "Messages:LR_BossSpawned" });
						plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameTextMessage) { Message = "Messages:LR_BossSpawned" });

						StartConversation(Target.World, 366542);

						if (plr.PlayerIndex == 0)
						{
							plr.SpawnNephalemBoss(Target.World);
						}
					}

				}

				if (Target.Quality > 1)
				{
					//spawn spheres for mining indicator
					for (int i = 0; i < Target.Quality + 1; i++)
					{
						var position = new Core.Types.Math.Vector3D(Target.Position.X + (float)RandomHelper.NextDouble() * 30f,
																	Target.Position.Y + (float)RandomHelper.NextDouble() * 30f,
																	Target.Position.Z);
						Item item = null;
						if (Target.World.Game.NephalemGreater)
							item = ItemGenerator.Cook(Master, "p1_tiered_rifts_Orb");
						else
							item = ItemGenerator.Cook(Master, "p1_normal_rifts_Orb");
						if (item != null)
							item.EnterWorld(position);
					}
				}
			}
			//Nephalem Rift Boss Killed
			if (Target.Attributes[GameAttribute.Is_Loot_Run_Boss])
			{
				Target.World.Game.ActiveNephalemKilledBoss = true;
				foreach (var plr in Target.World.Game.Players.Values)
				{
					//Enable banner /advocaite
					plr.Attributes[GameAttribute.Banner_Usable] = true;
					if (Target.World.Game.NephalemGreater)
					{
						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 16,
							TaskIndex = 0,
							Checked = 1,
							Counter = 1
						});
						plr.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 34,
							DisplayButton = true,
							Failed = false
						});

						plr.Attributes[GameAttribute.Jewel_Upgrades_Max] = 3;
						plr.Attributes[GameAttribute.Jewel_Upgrades_Used] = 0;
						plr.Attributes[GameAttribute.Jewel_Upgrades_Bonus] = 0;
						if (plr.Attributes[GameAttribute.Tiered_Loot_Run_Death_Count] == 0)
							plr.Attributes[GameAttribute.Jewel_Upgrades_Bonus]++;
						if (plr.InGameClient.Game.NephalemBuff)
							plr.Attributes[GameAttribute.Jewel_Upgrades_Bonus]++;

						plr.InGameClient.Game.LastTieredRiftTimeout = (int)((plr.InGameClient.Game.TiredRiftTimer.TimeoutTick - plr.InGameClient.Game.TickCounter) / plr.InGameClient.Game.TickRate / plr.InGameClient.Game.UpdateFrequency * 10f);
						plr.InGameClient.Game.TiredRiftTimer.Stop();
						plr.InGameClient.Game.TiredRiftTimer = null;

						plr.InGameClient.SendMessage(new DisplayGameTextMessage(Opcodes.DisplayGameTextMessage)
						{
							Message = "Messages:TieredRift_Success",
							Param1 = plr.InGameClient.Game.LastTieredRiftTimeout / 60, //Minutes
							Param2 = plr.InGameClient.Game.LastTieredRiftTimeout % 60 //Seconds
						});
						plr.InGameClient.SendMessage(new SNODataMessage(Opcodes.TimedEventResetMessage)
						{
							Field0 = 0x0005D6EA
						});

						Target.World.SpawnMonster(ActorSno._p1_lr_tieredrift_nephalem, Target.Position);

						Target.World.SpawnRandomUniqueGem(Target, plr);

						TagMap NewTagMap = new TagMap();
						NewTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, 332336, 0)); //World
						NewTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 332339, 0)); //Zone
						NewTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 24, 0)); //Entry-Pointа

						var portal = new Portal(Target.World, ActorSno._x1_openworld_lootrunportal, NewTagMap);

						portal.EnterWorld(new Core.Types.Math.Vector3D(Target.Position.X + 10f, Target.Position.Y + 10f, Target.Position.Z));
					}
					else
                    {
						plr.InGameClient.SendMessage(new QuestCounterMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 3,
							TaskIndex = 0,
							Checked = 1,
							Counter = 1
						});
						plr.InGameClient.SendMessage(new QuestUpdateMessage()
						{
							snoQuest = 0x00052654,
							snoLevelArea = 0x000466E2,
							StepID = 10,
							DisplayButton = true,
							Failed = false
						});
					}

					plr.InGameClient.SendMessage(new WorldSyncedDataMessage()
					{
						WorldID = Target.World.GlobalID,
						SyncedData = new WorldSyncedData()
						{
							SnoWeatherOverride = 362460,
							WeatherIntensityOverride = 0,
							WeatherIntensityOverrideEnd = 0
						}
					});
					//StartConversation(this.Target.World, 340878);
					var hubWorld = this.Target.World.Game.GetWorld(WorldSno.x1_tristram_adventure_mode_hub);
					var orek = (hubWorld.GetActorBySNO(ActorSno._x1_lr_nephalem) as InteractiveNPC);
					orek.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(340878));
					orek.ForceConversationSNO = 340878;
					orek.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					orek.Attributes[GameAttribute.Conversation_Icon, 1] = 2;
					orek.Attributes[GameAttribute.Conversation_Icon, 2] = 2;
					orek.Attributes[GameAttribute.Conversation_Icon, 3] = 2;
					orek.Attributes.BroadcastChangedIfRevealed();
					// Unique spawn
					Target.World.SpawnBloodShards(Target, plr, RandomHelper.Next(10, 30));
					Target.World.SpawnGold(Target, plr);
					Target.World.SpawnGold(Target, plr);
					Target.World.SpawnGold(Target, plr);
					plr.Toon.GameAccount.BigPortalKey++;
					Target.World.Game.ActiveNephalemProgress = 0f;
				}
			}

			if (Context != null)
			{
				if (Context.User is Player && Target.World.Game.MonsterLevel >= 70 && Context.User.Attributes[GameAttribute.Level] == 70) //keys
				{
					if (Target is Unique)
					{
						int chance = Target.World.Game.IsHardcore ? 30 : 10;
						if (Target.SNO != ActorSno._terrordemon_a_unique_1000monster && (Target as Unique).CanDropKey && FastRandom.Instance.Next(100) < chance)
							Target.World.DropItem(Target, null, ItemGenerator.CreateItem(Context.User, ItemGenerator.GetItemDefinition(-110888638)));
					}

					if (Target is Rare)
					{
						int chance = Target.World.Game.IsHardcore ? 15 : 5;
						if (FastRandom.Instance.Next(1000) < chance)
							Target.World.DropItem(Target, null, ItemGenerator.CreateItem(Context.User, ItemGenerator.GetItemDefinition(-110888638)));
					}
				}

				if (LootAndExp)
				{
					if (Context.User is Player || Context.User is Minion || Context.User is Hireling || Context.User == Target)
					{
						Player player = null;
						if (Context.User is Minion)
						{
							if ((Context.User as Minion).Master is Player)
								player = (Player)(Context.User as Minion).Master;
							else return;
						}
						else
						{
							if (Context.User is Player)
								player = (Player)Context.User;
						}

						if (player != null)
						{
							player.ExpBonusData.Update(player.GBHandle.Type, Target.GBHandle.Type);
							if (FastRandom.Instance.Next(1, 100) < 10)
								Target.World.SpawnHealthGlobe(Target, player, Target.Position);

							int chance = 2;         //Crusader -> Laws of Valor -> Answered Prayer
							if (player.World.BuffManager.HasBuff<CrusaderLawsOfValor.LawsApsBuff>(player))
								if (player.World.BuffManager.GetFirstBuff<CrusaderLawsOfValor.LawsApsBuff>(player).Glory)
									chance += 20;
							if (FastRandom.Instance.Next(1, 100) < chance)
								Target.World.SpawnPowerGlobe(Target, player, Target.Position);
						}

						//loot spawning
						foreach (var plr in Target.GetPlayersInRange(100))
						{
							if (FastRandom.Instance.NextDouble() < 0.45)
								Target.World.SpawnGold(Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.06)
								Target.World.SpawnRandomCraftItem(Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.04)
								Target.World.SpawnRandomGem(Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.15)
								Target.World.SpawnRandomPotion(Target, plr);
							if (Target.World.Game.Difficulty > 1)
								if (FastRandom.Instance.NextDouble() < 0.15)
									Target.World.SpawnItem(Target, plr, 2087837753);
							if (FastRandom.Instance.NextDouble() < 0.04)
								Target.World.SpawnRandomGem(Target, plr);
							//Logger.Debug("seed: {0}", seed);
							var dropRates = Target.World.Game.IsSeasoned ? LootManager.GetSeasonalDropRates((int)Target.Quality, Target.Attributes[GameAttribute.Level]) : LootManager.GetDropRates((int)Target.Quality, Target.Attributes[GameAttribute.Level]);

							float seed = (float)FastRandom.Instance.NextDouble();
							foreach (float rate in dropRates)
							{
								// if seed is less than the drop rate, drop the item
								if (seed < (rate * (1f 
								                    + plr.Attributes[GameAttribute.Magic_Find]) 
								                    * Config.Instance.RateDrop))
								{
									//Logger.Debug("rate: {0}", rate);
									var lootQuality = Target.World.Game.IsHardcore
										? LootManager.GetSeasonalLootQuality((int)Target.Quality,
											Target.World.Game.Difficulty)
										: LootManager.GetLootQuality((int)Target.Quality, Target.World.Game.Difficulty);
									Target.World.SpawnRandomEquip(Target, plr, lootQuality);
									if (Target is Goblin)
										Target.World.SpawnRandomGem(Target, plr);
								}
								else
									break;
							}

							if ((int)Target.Quality >= 4 && plr.AdditionalLootItems > 0)
								for (int d = 0; d < plr.AdditionalLootItems; d++)
								{
									var lootQuality = Target.World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)Target.Quality, Target.World.Game.Difficulty) : LootManager.GetLootQuality((int)Target.Quality, Target.World.Game.Difficulty);
									Target.World.SpawnRandomEquip(Target, plr, lootQuality);
								}

							if (Target is Champion or Rare or Unique or Boss)
							{
								//if (FastRandom.Instance.NextDouble() < LootManager.GetEssenceDropChance(this.Target.World.Game.Difficulty))
								//	this.Target.World.SpawnEssence(this.Target, plr);
								if (Target.World.Game.IsSeasoned)
									Target.World.SpawnBloodShards(Target, plr);
							}

							if (Target.World.Game.IsSeasoned)
                            {
								switch(Target.SNO)
                                {
									case ActorSno._despair: //Rakanot
										plr.GrantCriteria(74987254022737);
										break;
									case ActorSno._skeletonking: //Skillet King
										plr.GrantCriteria(74987252582955);
										break;
									case ActorSno._siegebreakerdemon: //Siegebreaker - Make your choice
										plr.GrantCriteria(74987246511881);
										break;
									case ActorSno._x1_adria_boss: //Adria - I become a star
										plr.GrantCriteria(74987252384014);
										break;
								}
                            }

                            if ((int)Target.Quality >= 4)
							{
								if (Target.SNO == ActorSno._lacunifemale_c_unique) //Chiltara
									if ((float)FastRandom.Instance.NextDouble() < 0.5f)
										Target.World.SpawnItem(Target, plr, -799974399);
								if (Target.SNO == ActorSno._bigred_izual) //Izual
									if ((float)FastRandom.Instance.NextDouble() < 0.2f)
									{
										switch (Target.World.Game.Difficulty)
										{
											case 0:
												Target.World.SpawnItem(Target, plr, -1463195022);
												break;
											case 1:
												Target.World.SpawnItem(Target, plr, 645585264);
												break;
											case 2:
												Target.World.SpawnItem(Target, plr, -501637898);
												break;
											case 3:
												Target.World.SpawnItem(Target, plr, 253048194);
												break;
											default:
												Target.World.SpawnItem(Target, plr, -1463195022);
												break;
										}
									}

								switch (Target.SNO)
								{
									case ActorSno._graverobber_a_ghost_unique_03:
										plr.GrantCriteria(74987243307212);
										break;
									case ActorSno._gravedigger_b_ghost_unique_01:
										plr.GrantCriteria(74987243309859);
										break;
									case ActorSno._graverobber_a_ghost_unique_01:
										plr.GrantCriteria(74987243309860);
										break;
									case ActorSno._graverobber_a_ghost_unique_02:
										plr.GrantCriteria(74987243309861);
										break;
									case ActorSno._ghost_a_unique_01:
										plr.GrantCriteria(74987243309862);
										break;
									case ActorSno._ghost_d_unique01:
										plr.GrantCriteria(74987243309863);
										break;
									case ActorSno._ghost_d_unique_01:
										plr.GrantCriteria(74987243309864);
										break;
								}
							}
						}
					}
				}

				if (Context.User is Player & Target is Monster)
					if (RandomHelper.Next(0, 100) > 40 & (Context.User as Player).Toon.Class == ToonClass.Necromancer)
					{
						var Flesh = Context.User.World.SpawnMonster(ActorSno._p6_necro_corpse_flesh, PositionOfDeath);
						Flesh.Attributes[GameAttribute.Necromancer_Corpse_Source_Monster_SNO] = (int)Target.SNO;
						Flesh.Attributes.BroadcastChangedIfRevealed();
					}
			}
			if (Target is Monster)
				(Target as Monster).PlayLore();

			bool isCoop = (Target.World.Game.Players.Count > 1);
			bool isHardcore = Target.World.Game.IsHardcore;
			bool isSeasoned = Target.World.Game.IsSeasoned;
			//114917

			if (Target.Quality == 7 || Target.Quality == 2 || Target.Quality == 4)
			{

			}

			if (Target is Boss)
				foreach (Player plr in players)
					switch (Target.SNO)
					{
						case ActorSno._skeletonking: //Leoric
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987252301189); if (isHardcore) plr.GrantAchievement(74987243307489); else plr.GrantAchievement(74987249381288);
							break;
						case ActorSno._butcher: //Butcher
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243307946);
							if (isCoop) plr.GrantAchievement(74987252696819); if (isHardcore) plr.GrantAchievement(74987254551339); else plr.GrantAchievement(74987258164419);
							plr.SetProgress(1, Target.World.Game.Difficulty);
							break;
						case ActorSno._maghda: //Maghda
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987255855515); if (isHardcore) plr.GrantAchievement(74987243307507); else plr.GrantAchievement(74987246434969);
							break;
						case ActorSno._zoltunkulle: //Zoltun Kulle
							if (isCoop) plr.GrantAchievement(74987246137208); if (isHardcore) plr.GrantAchievement(74987243307509); else plr.GrantAchievement(74987252195665);
							break;
						case ActorSno._belial: //Belial (big)
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243310916);
							if (isCoop) plr.GrantAchievement(74987256826382); if (isHardcore) plr.GrantAchievement(74987244906887); else plr.GrantAchievement(74987244645044);
							plr.SetProgress(2, Target.World.Game.Difficulty);
							break;
						case ActorSno._gluttony: //Gluttony
							if (isCoop) plr.GrantAchievement(74987249112946); if (isHardcore) plr.GrantAchievement(74987243307519); else plr.GrantAchievement(74987259418615);
							break;
						case ActorSno._siegebreakerdemon: //Siegebreaker
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987253664242); if (isHardcore) plr.GrantAchievement(74987243307521); else plr.GrantAchievement(74987248255991);
							break;
						case ActorSno._mistressofpain: //Cydaea
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987257890442); if (isHardcore) plr.GrantAchievement(74987243307523); else plr.GrantAchievement(74987254675042);
							break;
						case ActorSno._azmodan: //Azmodan
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243310915);
							if (isCoop) plr.GrantAchievement(74987247100576); if (isHardcore) plr.GrantAchievement(74987251893684); else plr.GrantAchievement(74987247855713);
							plr.SetProgress(3, Target.World.Game.Difficulty);
							break;
						case ActorSno._terrordemon_a_unique_1000monster: //Iskatu
							if (isCoop) plr.GrantAchievement(74987255392558); if (isHardcore) plr.GrantAchievement(74987248632930); else plr.GrantAchievement(74987246017001);
							break;
						case ActorSno._despair: //Rakanoth
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987248781143); if (isHardcore) plr.GrantAchievement(74987243307533); else plr.GrantAchievement(74987256508058);
							break;
						case ActorSno._bigred_izual: //Izual
							if (isCoop) plr.GrantAchievement(74987254969009); if (isHardcore) plr.GrantAchievement(74987247989681); else plr.GrantAchievement(74987244988685);
							if (isSeasoned) plr.GrantCriteria(74987249642121);
							break;
						case ActorSno._diablo: //Diablo
							if (Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987250386944); if (isHardcore) plr.GrantAchievement(74987250070969); else plr.GrantAchievement(74987248188984);
							plr.SetProgress(4, Target.World.Game.Difficulty);
							if (isSeasoned) plr.GrantCriteria(74987250915380);
							break;
						default:
							break;
					}
		}

		public bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var plr in world.Players)
				plr.Value.Conversations.StartConversation(conversationId);
			return true;
		}

		private void DoPlayerDeath()
		{
			//death implementation
			Player player = (Player)Target;
			if (Math.Abs(player.Attributes[GameAttribute.Item_Power_Passive, 248629] - 1) < 0.001)
				player.PlayEffectGroup(248680);
			player.StopCasting();
			Target.World.BuffManager.RemoveAllBuffs(Target, false);
			Target.World.PowerManager.CancelAllPowers(Target);

			//player.Dead = true;
			player.InGameClient.SendMessage(new VictimMessage()
			{
				PlayerVictimIndex = player.PlayerIndex, //player victim
				KillerLevel = 100,
				KillerPlayerIndex = (Context.User is Player ? (Context.User as Player).PlayerIndex : -1),                                                                            //player killer(?)
				KillerMonsterRarity = (Context.User is Player ? 0 : (int)Context.User.Quality),            //quality of actorKiller
				snoKillerActor = Context.User is Player ? -1 : (int)Context.User.SNO,    //if player killer, then minion SnoId
				KillerTeam = -1,                                                                            //player killer(?)
				KillerRareNameGBIDs = new int[] { -1, -1 },
				snoPowerDmgSource = -1
			});

			//player.PlayAnimation(11, this.Target.AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault], 1f);
			if (!player.World.Game.PvP)
			{
				Target.World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
				{
					ActorID = Target.DynamicID(plr),
					AnimationSNO = AnimationSetKeys.DeadDefault.ID
				}, Target);

				if (!player.World.Game.IsHardcore)
					player.InGameClient.SendMessage(new DeathPlayerMesage(Opcodes.DeathPlayerMesage) { PlayerIndex = player.PlayerIndex, });
			}
			else
				if (!player.World.Game.Players.Values.Any(p => p.Attributes[GameAttribute.TeamID] == player.Attributes[GameAttribute.TeamID] && !p.Dead))
				player.World.Game.StartPvPRound();

			/*if (player.World.Game.IsHardcore)
			{
				player.Toon.Flags = player.Toon.Flags | ToonFlags.Fallen;
			} else
			{*/
			//spawning tomb
			player.QueueDeath(true);
			if (!player.World.IsPvP)
			{
				var tomb = new Headstone(Target.World, ActorSno._playerheadstone, new TagMap(), player.PlayerIndex);
				tomb.EnterWorld(player.Position);

				player.Inventory.DecreaseDurability(0.1f);
				if (player.World.Game.IsHardcore)
				{
					player.AddTimedAction(3f, new Action<int>((q) => player.Revive(player.CheckPointPosition)));
					var toon = player.Toon.DBToon;
					toon.Deaths++;
					player.World.Game.GameDbSession.SessionUpdate(toon);
				}
			}
			//}
		}

		private AnimationSno FindBestDeathAnimationSNO()
		{
			if (Context == null)
                return AnimationSno._NONE;

            // check if power has special death animation, and roll chance to use it
            TagKeyInt specialDeathTag = GetTagForSpecialDeath(Context.EvalTag(PowerKeys.SpecialDeathType));
            if (specialDeathTag != null)
            {
                float specialDeathChance = Context.EvalTag(PowerKeys.SpecialDeathChance);
                if (PowerContext.Rand.NextDouble() < specialDeathChance)
                {
                    var specialSNO = GetSNOFromTag(specialDeathTag);
                    if (specialSNO != AnimationSno._NONE)
                    {
                        return specialSNO;
                    }
                }
                // decided not to use special death or actor doesn't have it, just fall back to normal death anis
            }

            var sno = GetSNOFromTag(this.DeathDamageType.DeathAnimationTag);
            if (sno != AnimationSno._NONE)
                return sno;

            //if (this.Target.ActorSNO.Name.Contains("Spiderling")) return _GetSNOFromTag(new TagKeyInt(69764));

            //Logger.Debug("monster animations:");
            //foreach (var anim in this.Target.AnimationSet.TagMapAnimDefault)
            //	Logger.Debug("animation: {0}", anim.ToString());

            // load default ani if all else fails
            return GetSNOFromTag(AnimationSetKeys.DeathDefault);
        }

		private AnimationSno GetSNOFromTag(TagKeyInt tag)
		{
			if (Target.AnimationSet != null && Target.AnimationSet.TagMapAnimDefault.ContainsKey(tag))
				return (AnimationSno)Target.AnimationSet.TagMapAnimDefault[tag];
			else
				return AnimationSno._NONE;
		}

		private static TagKeyInt GetTagForSpecialDeath(int specialDeathType)
		{
			switch (specialDeathType)
			{
				default: return null;
				case 1: return AnimationSetKeys.DeathDisintegration;
				case 2: return AnimationSetKeys.DeathPulverise;
				case 3: return AnimationSetKeys.DeathPlague;
				case 4: return AnimationSetKeys.DeathDismember;
				case 5: return AnimationSetKeys.DeathDecap;
				case 6: return AnimationSetKeys.DeathAcid;
				case 7: return AnimationSetKeys.DeathLava;  // haven't seen lava used, but there's no other place for it
				case 8: return AnimationSetKeys.DeathSpirit;
			}
		}
	}
}

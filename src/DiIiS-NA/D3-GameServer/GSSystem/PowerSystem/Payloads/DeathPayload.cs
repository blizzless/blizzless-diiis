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
			this.LootAndExp = grantsLootAndExp;
			this.DeathDamageType = deathDamageType;

			if (this.Target == null)
			{
				if (target == null)
					return;
				else
					this.Target = target;
			}
			if (this.Target.World == null) return;
			if (!this.Target.Visible) return;
			if (!this.Target.World.Game.Working) return;
			if (this.Target.Dead)
				return;

			if (this.Target is Player)
			{
				var plr = this.Target as Player;
				if(this.Target.World.Game.NephalemGreater)
					(this.Target as Player).Attributes[GameAttribute.Tiered_Loot_Run_Death_Count]++;
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
			if (this.Target is Hireling)
			{
				Hireling mon = (Hireling)this.Target;
				mon.Dead = true;

				if (mon.Dead)
				{
					mon.Attributes[GameAttribute.Hitpoints_Cur] = mon.Attributes[GameAttribute.Hitpoints_Max_Total];
					mon.Attributes.BroadcastChangedIfRevealed();
					mon.Dead = false;
				}

				return;
			}
			this.Successful = true;
		}

		public void Apply()
		{
			var PositionOfDeath = this.Target.Position;
			if (!this.Target.World.Game.Working) return;

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

			if (this.Target is NecromancerSkeleton_A)
			{
				//(this.Target as NecromancerSkeleton_A).Master+
				((this.Target as NecromancerSkeleton_A).Master as Player).InGameClient.SendMessage(new MessageSystem.Message.Definitions.Pet.PetDetachMessage()
				{
					PetId = this.Target.DynamicID(((this.Target as NecromancerSkeleton_A).Master as Player))
				});
				((this.Target as NecromancerSkeleton_A).Master as Player).NecroSkeletons.Remove(this.Target);
			}
			if (this.Target is BaseGolem ||
					this.Target is IceGolem ||
					this.Target is BoneGolem ||
					this.Target is DecayGolem ||
					this.Target is ConsumeFleshGolem ||
					this.Target is BloodGolem)
            {
				((this.Target as Minion).Master as Player).InGameClient.SendMessage(new MessageSystem.Message.Definitions.Pet.PetDetachMessage()
				{
					PetId = this.Target.DynamicID(((this.Target as Minion).Master as Player))
				});
				((this.Target as Minion).Master as Player).ActiveGolem = null;
			}
			if (this.Target is Player)
			{
				var plr = this.Target as Player;

				if (plr.SkillSet.HasPassive(208779)) //Grenadier (DH)
				{
					plr.World.PowerManager.RunPower(plr, 208779);
				}

				plr.Attributes.BroadcastChangedIfRevealed();

				DoPlayerDeath();
				return;
			}

			if (this.Context != null)
				if (this.Context.User is Player)  //Hitpoints_On_Kill
					if (this.Context.User.Attributes[GameAttribute.Hitpoints_On_Kill] > 0)
						(this.Context.User as Player).AddHP(this.Context.User.Attributes[GameAttribute.Hitpoints_On_Kill]);

			// HACK: add to hackish list thats used to defer deleting actor and filter it from powers targetting
			if (!(this.Target is Boss))
				this.Target.World.PowerManager.AddDeletingActor(this.Target);

			if (this.Target is Living)
			{
				Living actor = (Living)this.Target;
				if (actor.Brain != null)
					actor.Brain.Kill();
			}

			if (this.Target is Monster)
			{

				Monster mon = (Monster)this.Target;
				if (mon.Brain != null)
					mon.Brain.Kill();

				mon.World.BroadcastIfRevealed(plr => new ACDCollFlagsMessage
				{
					ActorID = mon.DynamicID(plr),
					CollFlags = 0
				}, mon);

			}

			if (this.Target is Minion)
			{
				Minion mon = (Minion)this.Target;
				if (mon.Master != null && mon.Master is Player)
				{
					(mon.Master as Player).Followers.Remove(this.Target.GlobalID);
					(mon.Master as Player).FreeFollowerIndex(mon.SNO);
				}
				if (mon.Brain != null)
					mon.Brain.Kill();

				this.LootAndExp = false;
			}
			bool championsAlive = false;

			if (this.Target is Champion)
			{
				championsAlive = this.Target.GetActorsInRange<Champion>(1000).Where(c => c.GroupId == this.Target.GroupId && c.Attributes[GameAttribute.Hitpoints_Cur] > 0).ToList().Count > 0;
				if (championsAlive)
					this.LootAndExp = false;
			}

			// send this death payload to buffs
			this.Target.World.BuffManager.SendTargetPayload(this.Target, this);

			if (this.Context != null)
				if (this.Context.User != null)
					this.Target.World.BuffManager.SendTargetPayload(this.Context.User, this);

			this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage()
			{
				ActorId = this.Target.DynamicID(plr),
				Effect = Effect.Unknown12,
			}, this.Target);

			this.Target.World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.PlayIdleAnimationMessage)
			{
				ActorID = this.Target.DynamicID(plr)
			}, this.Target);

			// special death animation
			switch (this.Target.SNO)
			{
				//Boss-A1 Q2
				case ActorSno._skeleton_a_cain_unique: this.Target.PlayAnimation(11, 199484, 1f); break;
				//Йондар
				case ActorSno._adventurer_d_templarintrounique: this.Target.PlayAnimation(11, 199484, 1f); break;
				//Разнощик чумы
				case ActorSno._fleshpitflyer_b: this.Target.PlayAnimation(11, 8535, 1f); break;
				//Темные жрецы
				case ActorSno._triunevessel_event31: this.Target.PlayAnimation(11, 199484, 1f); break;
				//Пчелы
				case ActorSno._sandwasp_a:
				case ActorSno._fleshpitflyer_leoric_inferno:
					this.Target.PlayAnimation(11, 8535, 1f);
					break;
				//X1_LR_Boss_Angel_Corrupt_A
				case ActorSno._x1_lr_boss_angel_corrupt_a: this.Target.PlayAnimation(11, 142005, 1f); break;
				//Падшие
				case ActorSno._fallengrunt_a: this.Target.PlayAnimation(11, 199484, 1f); break;
				default:
					if (_FindBestDeathAnimationSNO() != -1)
						this.Target.PlayAnimation(11, _FindBestDeathAnimationSNO(), 1f);
					else
					{
						Logger.Warn("Анимация смерти не обнаружена: ActorSNOId = {0}", Target.SNO);
					}
					break;
			}

			this.Target.World.BroadcastIfRevealed(plr => new ANNDataMessage(Opcodes.CancelACDTargetMessage)
			{
				ActorID = this.Target.DynamicID(plr),
			}, this.Target);


			// remove all buffs and running powers before deleting actor
			this.Target.World.BuffManager.RemoveAllBuffs(this.Target);
			this.Target.World.PowerManager.CancelAllPowers(this.Target);

			this.Target.Attributes[GameAttribute.Deleted_On_Server] = true;
			this.Target.Attributes[GameAttribute.Could_Have_Ragdolled] = true;
			this.Target.Attributes.BroadcastChangedIfRevealed();

			this.Target.World.BroadcastIfRevealed(plr => new DeathFadeTimeMessage()
			{
				Field0 = this.Target.DynamicID(plr),
				Field1 = 300,
				Field2 = 200,
				Field3 = true
			}, this.Target);

			if (this.Context != null)
				if (this.Context.User.Attributes[GameAttribute.Item_Power_Passive, 247640] == 1 ||
					this.Context.User.Attributes[GameAttribute.Item_Power_Passive, 249963] == 1 ||
					this.Context.User.Attributes[GameAttribute.Item_Power_Passive, 249954] == 1 ||
					(float)FastRandom.Instance.NextDouble() < 0.1f ||
					this.Target.World.SNO == WorldSno.a1dun_random_level01)
					switch ((int)this.DeathDamageType.HitEffect)
					{
						case 0: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.Gore }, this.Target); break;
						case 1: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.GoreFire }, this.Target); break;
						case 2: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.GoreElectro }, this.Target); break;
						case 3: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.IceBreak }, this.Target); break;
						case 4: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.GorePoison }, this.Target); break;
						case 5: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.GoreArcane }, this.Target); break;
						case 6: this.Target.World.BroadcastIfRevealed(plr => new PlayEffectMessage() { ActorId = this.Target.DynamicID(plr), Effect = Effect.GoreHoly }, this.Target); break;
					}

			if (this.Context != null)
				if (this.Context.User is Player && Math.Abs(this.Context.User.Attributes[GameAttribute.Level] - this.Target.Attributes[GameAttribute.Level]) < 5)
					(this.Context.User as Player).KilledSeasonalTempCount++;

			if (this.Context.User is Player)
				if (this.Context.World.BuffManager.HasBuff<LandOfDead.ZBuff>(this.Context.User))
				{
					(this.Context.User as Player).BuffStreakKill += 1;
					if ((this.Context.User as Player).BuffStreakKill == 10 || (this.Context.User as Player).BuffStreakKill == 20)
					{
						//(this.Context.User as Player).Attributes[_Buff_Icon_End_TickN, PowerSNO]
					}
				}


			if (this.Target.SNO == ActorSno._a4dun_garden_corruption_monster) //Сады надежды
			{
				//Первый этаж садов надежды
				if (Target.World.SNO == WorldSno.a4dun_garden_of_hope_01)
				{
					//Проверяем есть ли порталы
					var PortalToHell = Target.World.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal); //{[Actor] [Type: Gizmo] SNOId:224890 DynamicId: 280 Position: x:696,681 y:695,4387 z:0,2636871 Name: a4_Heaven_Gardens_HellPortal}
					if (PortalToHell.Count == 0)
					{
						var Corruptions = Target.World.GetActorsBySNO(ActorSno._a4dun_garden_corruption_monster);
						if (Corruptions.Count > 1)
						{
							if (RandomHelper.Next(0, 30) > 26)
							{
								Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
								HellPortal.EnterWorld(this.Target.Position);
								this.Context.User.World.SpawnMonster(ActorSno._diablo_vo, this.Context.User.Position);
								StartConversation(Target.World, 217226);
							}
						}
						else
						{
							Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
							HellPortal.EnterWorld(this.Target.Position);
							this.Context.User.World.SpawnMonster(ActorSno._diablo_vo, this.Context.User.Position);
							StartConversation(Target.World, 217226);
						}
					}
				}
				//Второй этаж садов надежды
				else if (Target.World.SNO == WorldSno.a4dun_garden_of_hope_random)
				{ //Проверяем есть ли порталы
					var PortalToHell = Target.World.GetActorsBySNO(ActorSno._a4_heaven_gardens_hellportal); //{[Actor] [Type: Gizmo] SNOId:224890 DynamicId: 280 Position: x:696,681 y:695,4387 z:0,2636871 Name: a4_Heaven_Gardens_HellPortal}
					if (PortalToHell.Count == 0)
					{
						var Corruptions = Target.World.GetActorsBySNO(ActorSno._a4dun_garden_corruption_monster);
						if (Corruptions.Count > 1)
						{
							if (RandomHelper.Next(0, 30) > 26)
							{
								Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
								HellPortal.EnterWorld(this.Target.Position);
								if (this.Context.User.World.GetActorsBySNO(ActorSno._diablo_vo).Count == 0)
									this.Context.User.World.SpawnMonster(ActorSno._diablo_vo, this.Context.User.Position);
								StartConversation(Target.World, 217228);
							}
						}
						else
						{
							Portal HellPortal = new Portal(Target.World, ActorSno._a4_heaven_gardens_hellportal, Target.World.StartingPoints[0].Tags);
							HellPortal.EnterWorld(this.Target.Position);
							if (this.Context.User.World.GetActorsBySNO(ActorSno._diablo_vo).Count == 0)
								this.Context.User.World.SpawnMonster(ActorSno._diablo_vo, this.Context.User.Position);
							StartConversation(Target.World, 217228);
						}
					}
				}
			}

			// Spawn Random item and give exp for each player in range
			List<Player> players = this.Target.GetPlayersInRange(100f);
			foreach (Player plr in players)
			{
				int grantedExp = 0;
				if (plr.Attributes[GameAttribute.Level] <= this.Target.Attributes[GameAttribute.Level])
					grantedExp = (int)(Player.LevelBorders[plr.Attributes[GameAttribute.Level]] / (40 * this.Target.Attributes[GameAttribute.Level] * 0.85f) * (this.Target is Monster ? Math.Min((this.Target as Monster).HPMultiplier, 3f) : 1f));
				else
					grantedExp = (int)(Player.LevelBorders[plr.Attributes[GameAttribute.Level]] / (40 * this.Target.Attributes[GameAttribute.Level] * 0.85f) * (1 - Math.Abs(plr.Attributes[GameAttribute.Level] - this.Target.Attributes[GameAttribute.Level]) / 20));

				grantedExp = (int)(grantedExp * (plr.Attributes[GameAttribute.Experience_Bonus_Percent] + 1));
				grantedExp += (int)plr.Attributes[GameAttribute.Experience_Bonus];



				if (LootAndExp)
				{
					grantedExp = (int)(grantedExp * plr.World.Game.XPModifier);

					float tempEXP = grantedExp * DiIiS_NA.GameServer.Config.Instance.RateEXP;

					plr.UpdateExp(Math.Max((int)tempEXP, 1));
					var a = (int)plr.Attributes[GameAttribute.Experience_Bonus];
					var a1 = (int)plr.Attributes[GameAttribute.Experience_Bonus_Percent];

					plr.KilledMonstersTempCount++;
				}

				if (this.Target is Champion || this.Target is Rare || this.Target is Boss || this.Target is Unique)
				{
					plr.KilledElitesTempCount++;
				}

				//achievements here
				if (this.Target is Monster)
				{
					if (plr.Toon.Class == ToonClass.DemonHunter)
					{
						if ((this.Target as Monster).MonsterType == (int)DiIiS_NA.Core.MPQ.FileFormats.Monster.MonsterType.Demon)
							plr.AddAchievementCounter(74987243307065, 1);

						if (PowerMath.Distance2D(plr.Position, this.Target.Position) >= 45f)
							plr.AddAchievementCounter(74987243307061, 1);

						if (this.Target.Attributes[GameAttribute.Feared] == true)
							plr.AddAchievementCounter(74987243307064, 1);

						if (this.Context.PowerSNO == 75301)
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
						if (this.Target.Attributes[GameAttribute.Frozen] == true)
							plr.AddAchievementCounter(74987243307585, 1);
					}
					if (plr.Toon.Class == ToonClass.WitchDoctor)
					{
						if (this.Context.User.Attributes[GameAttribute.Team_Override] == 1)
							plr.AddAchievementCounter(74987243307564, 1);
					}

					if (this.Target is Champion)
					{
						plr.CheckKillMonsterCriteria(this.Target.SNO, 1);
					}
					if (this.Target is Rare)
					{
						plr.CheckKillMonsterCriteria(this.Target.SNO, 2);
					}
					if (this.Target is Unique)
					{
						plr.CheckKillMonsterCriteria(this.Target.SNO, 4);
					}
				}

				if (this.Target is Unique)
				{
					if (LoreRegistry.Lore.ContainsKey(this.Target.World.SNO) && LoreRegistry.Lore[this.Target.World.SNO].chests_lore.ContainsKey(this.Target.SNO))
						foreach (int loreId in LoreRegistry.Lore[this.Target.World.SNO].chests_lore[this.Target.SNO])
							if (!plr.HasLore(loreId))
							{
								this.Target.World.DropItem(this.Target, null, ItemGenerator.CreateLore(plr, loreId));
								break;
							}
				}

				if (plr.SkillSet.HasPassive(218191) && PowerMath.Distance2D(plr.Position, this.Target.Position) <= 20f + plr.Attributes[GameAttribute.Gold_PickUp_Radius]) //GraveInjustice (WD)
				{
					plr.AddHP(plr.Attributes[GameAttribute.Hitpoints_Max_Total] / 100f);
					plr.GeneratePrimaryResource(plr.Attributes[GameAttribute.Resource_Max_Total, 0] / 100f);
					foreach (var cdBuff in plr.World.BuffManager.GetBuffs<PowerSystem.Implementations.CooldownBuff>(plr))
						cdBuff.Reduce(60);
				}

				if (plr.SkillSet.HasPassive(357218) && PowerMath.Distance2D(plr.Position, this.Target.Position) <= 15f) //Fervor (Crusader)
				{
					plr.World.BuffManager.AddBuff(plr, plr, new FervorBuff());
				}

				if (this.Target.World.BuffManager.HasBuff<Leech.Rune_E_Buff>(this.Target))
					foreach (var a in this.Target.World.BuffManager.GetBuffs<CooldownBuff>(this.Context.User))
						if (a.TargetPowerSNO == 30211)
							a.Reduce(60);

				if (this.Target.World.BuffManager.HasBuff<Leech.Rune_C_Buff>(this.Target))
				{
					this.Context.User.AddHP(this.Context.User.Attributes[GameAttribute.Hitpoints_On_Kill_Total] * 2f);
				}


				if (this.Target.World.BuffManager.HasBuff<CrusaderJudgment.JudgedDebuffRooted>(this.Target))    //Crusader -> Judgment -> Conversion
					if (this.Target.World.BuffManager.GetFirstBuff<CrusaderJudgment.JudgedDebuffRooted>(this.Target).conversion)
						if (FastRandom.Instance.Next() < 0.2f)
						{
							var avatar = new AvatarMelee(plr.World, this.Context, 0, 1f, this.Context.WaitSeconds(7f));
							avatar.Brain.DeActivate();
							avatar.Position = PowerContext.RandomDirection(plr.Position, 3f, 8f);
							avatar.Attributes[GameAttribute.Untargetable] = true;

							avatar.EnterWorld(avatar.Position);

							System.Threading.Tasks.Task.Delay(1000).ContinueWith(d =>
							{
								(avatar as Minion).Brain.Activate();
								avatar.Attributes[GameAttribute.Untargetable] = false;
								avatar.Attributes.BroadcastChangedIfRevealed();
							});
						}

				if (plr.SkillSet.HasPassive(208571) && PowerMath.Distance2D(plr.Position, this.Target.Position) <= 12f + plr.Attributes[GameAttribute.Gold_PickUp_Radius] && FastRandom.Instance.Next(100) < 5) //CircleOfLife (WD)
				{
					var dog = new ZombieDog(plr.World, plr, 0);
					dog.Brain.DeActivate();
					dog.Position = PowerContext.RandomDirection(plr.Position, 3f, 8f);
					dog.Attributes[GameAttribute.Untargetable] = true;
					dog.EnterWorld(dog.Position);
					dog.PlayActionAnimation(11437);
					this.Context.DogsSummoned++;

					System.Threading.Tasks.Task.Delay(1000).ContinueWith(d =>
					{
						dog.Brain.Activate();
						dog.Attributes[GameAttribute.Untargetable] = false;
						dog.Attributes.BroadcastChangedIfRevealed();
						dog.PlayActionAnimation(11431);
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
				if (this.Context != null)
					if (this.Context.DogsSummoned >= 3)
						plr.GrantAchievement(74987243307567);
			}
			Logger.Trace("Killed monster, id: {0}, level {1}", this.Target.SNO, this.Target.Attributes[GameAttribute.Level]);


			//handling quest triggers
			if (this.Target.World.Game.QuestProgress.QuestTriggers.ContainsKey((int)this.Target.SNO))
			{
				var trigger = this.Target.World.Game.QuestProgress.QuestTriggers[(int)this.Target.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					this.Target.World.Game.QuestProgress.UpdateCounter((int)this.Target.SNO);
					if (trigger.count == this.Target.World.Game.QuestProgress.QuestTriggers[(int)this.Target.SNO].counter)
						trigger.questEvent.Execute(this.Target.World); // launch a questEvent
				}
				else
					if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.MonsterFromGroup)
				{
					this.Target.World.Game.QuestProgress.UpdateCounter((int)this.Target.SNO);
				}
			}
			else if (this.Target.World.Game.SideQuestProgress.QuestTriggers.ContainsKey((int)this.Target.SNO))
			{
				var trigger = this.Target.World.Game.SideQuestProgress.QuestTriggers[(int)this.Target.SNO];
				if (trigger.triggerType == DiIiS_NA.Core.MPQ.FileFormats.QuestStepObjectiveType.KillMonster)
				{
					this.Target.World.Game.SideQuestProgress.UpdateSideCounter((int)this.Target.SNO);
					if (trigger.count == this.Target.World.Game.SideQuestProgress.QuestTriggers[(int)this.Target.SNO].counter)
						trigger.questEvent.Execute(this.Target.World); // launch a questEvent
				}
			}
			if (Target.World == null)
				return;
			foreach (var bounty in this.Target.World.Game.QuestManager.Bounties)
			{	if (this.Target.OriginalLevelArea == -1)
					this.Target.OriginalLevelArea = this.Target.CurrentScene.Specification.SNOLevelAreas[0];
				bounty.CheckKill((int)this.Target.SNO, this.Target.OriginalLevelArea, this.Target.World.SNO);
			}

			//Nephalem Rift
			if ((this.Target.CurrentScene.Specification.SNOLevelAreas[0] == 332339 || this.Target.CurrentScene.Specification.SNOLevelAreas[0] == 288482) && this.Target.World.Game.ActiveNephalemTimer == true && this.Target.World.Game.ActiveNephalemKilledMobs == false)
			{
				this.Target.World.Game.ActiveNephalemProgress += (1f * (this.Target.Quality + 1));
				Player Master = null;
				foreach (var plr in this.Target.World.Game.Players.Values)
				{
					if (plr.PlayerIndex == 0)
						Master = plr;
					plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.KillCounterRefresh)
					{

					});

					plr.InGameClient.SendMessage(new FloatDataMessage(Opcodes.DungeonFinderProgressMessage)
					{
						Field0 = this.Target.World.Game.ActiveNephalemProgress
					});



					if (this.Target.World.Game.ActiveNephalemProgress > 650)
					{
						this.Target.World.Game.ActiveNephalemKilledMobs = true;
						if (this.Target.World.Game.NephalemGreater)
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

						this.StartConversation(this.Target.World, 366542);

						if (plr.PlayerIndex == 0)
						{
							plr.SpawnNephalemBoss(this.Target.World);
						}
					}

				}

				if (this.Target.Quality > 1)
				{
					//Спауним сферы для майна показателя
					for (int i = 0; i < this.Target.Quality + 1; i++)
					{
						var position = new Core.Types.Math.Vector3D(this.Target.Position.X + (float)RandomHelper.NextDouble() * 30f,
																	this.Target.Position.Y + (float)RandomHelper.NextDouble() * 30f,
																	this.Target.Position.Z);
						Item item = null;
						if (this.Target.World.Game.NephalemGreater)
							item = ItemGenerator.Cook(Master, "p1_tiered_rifts_Orb");
						else
							item = ItemGenerator.Cook(Master, "p1_normal_rifts_Orb");
						if (item != null)
							item.EnterWorld(position);
					}
				}
			}
			//Nephalem Rift Boss Killed
			if (this.Target.Attributes[GameAttribute.Is_Loot_Run_Boss] == true)
			{
				this.Target.World.Game.ActiveNephalemKilledBoss = true;
				foreach (var plr in this.Target.World.Game.Players.Values)
				{
					//Enable banner /advocaite
          plr.Attributes[GameAttributeB.Banner_Usable] = true;
					if (this.Target.World.Game.NephalemGreater)
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
						if (plr.InGameClient.Game.NephalemBuff == true)
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

						this.Target.World.SpawnMonster(ActorSno._p1_lr_tieredrift_nephalem, this.Target.Position);

						this.Target.World.SpawnRandomUniqueGem(this.Target, plr);

						TagMap NewTagMap = new TagMap();
						NewTagMap.Add(new TagKeySNO(526850), new TagMapEntry(526850, 332336, 0)); //Мир
						NewTagMap.Add(new TagKeySNO(526853), new TagMapEntry(526853, 332339, 0)); //Зона
						NewTagMap.Add(new TagKeySNO(526851), new TagMapEntry(526851, 24, 0)); //Точка входа

						var portal = new Portal(this.Target.World, ActorSno._x1_openworld_lootrunportal, NewTagMap);

						portal.EnterWorld(new Core.Types.Math.Vector3D(this.Target.Position.X + 10f, this.Target.Position.Y + 10f, this.Target.Position.Z));
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
						WorldID = this.Target.World.GlobalID,
						SyncedData = new WorldSyncedData()
						{
							SnoWeatherOverride = 362460,
							WeatherIntensityOverride = 0,
							WeatherIntensityOverrideEnd = 0
						}
					});
					//StartConversation(this.Target.World, 340878);
					var HubWorld = this.Target.World.Game.GetWorld(WorldSno.x1_tristram_adventure_mode_hub);
					var Orek = (HubWorld.GetActorBySNO(ActorSno._x1_lr_nephalem) as InteractiveNPC);
					Orek.Conversations.Add(new ActorSystem.Interactions.ConversationInteraction(340878));
					Orek.ForceConversationSNO = 340878;
					Orek.Attributes[GameAttribute.Conversation_Icon, 0] = 2;
					Orek.Attributes[GameAttribute.Conversation_Icon, 1] = 2;
					Orek.Attributes[GameAttribute.Conversation_Icon, 2] = 2;
					Orek.Attributes[GameAttribute.Conversation_Icon, 3] = 2;
					Orek.Attributes.BroadcastChangedIfRevealed();
					//Уникальный спавн
					this.Target.World.SpawnBloodShards(this.Target, plr, RandomHelper.Next(10, 30));
					this.Target.World.SpawnGold(this.Target, plr);
					this.Target.World.SpawnGold(this.Target, plr);
					this.Target.World.SpawnGold(this.Target, plr);
					plr.Toon.BigPortalKey++;
					this.Target.World.Game.ActiveNephalemProgress = 0f;
				}
			}

			if (this.Context != null)
			{
				if (this.Context.User is Player && this.Target.World.Game.MonsterLevel >= 70 && this.Context.User.Attributes[GameAttribute.Level] == 70) //keys
				{
					if (this.Target is Unique)
					{
						int chance = this.Target.World.Game.IsHardcore ? 30 : 10;
						if (this.Target.SNO != ActorSno._terrordemon_a_unique_1000monster && (this.Target as Unique).CanDropKey && FastRandom.Instance.Next(100) < chance)
							this.Target.World.DropItem(this.Target, null, ItemGenerator.CreateItem(this.Context.User, ItemGenerator.GetItemDefinition(-110888638)));
					}

					if (this.Target is Rare)
					{
						int chance = this.Target.World.Game.IsHardcore ? 15 : 5;
						if (FastRandom.Instance.Next(1000) < chance)
							this.Target.World.DropItem(this.Target, null, ItemGenerator.CreateItem(this.Context.User, ItemGenerator.GetItemDefinition(-110888638)));
					}
				}

				if (LootAndExp)
				{
					if (this.Context.User is Player || this.Context.User is Minion || this.Context.User is Hireling || this.Context.User == this.Target)
					{
						Player player = null;
						if (this.Context.User is Minion)
						{
							if ((this.Context.User as Minion).Master is Player)
								player = (Player)(this.Context.User as Minion).Master;
							else return;
						}
						else
						{
							if (this.Context.User is Player)
								player = (Player)this.Context.User;
						}

						if (player != null)
						{
							player.ExpBonusData.Update(player.GBHandle.Type, this.Target.GBHandle.Type);
							if (FastRandom.Instance.Next(1, 100) < 10)
								this.Target.World.SpawnHealthGlobe(this.Target, player, this.Target.Position);

							int chance = 2;         //Crusader -> Laws of Valor -> Answered Prayer
							if (player.World.BuffManager.HasBuff<CrusaderLawsOfValor.LawsApsBuff>(player))
								if (player.World.BuffManager.GetFirstBuff<CrusaderLawsOfValor.LawsApsBuff>(player).Glory)
									chance += 20;
							if (FastRandom.Instance.Next(1, 100) < chance)
								this.Target.World.SpawnPowerGlobe(this.Target, player, this.Target.Position);
						}

						//loot spawning
						foreach (var plr in this.Target.GetPlayersInRange(100))
						{
							if (FastRandom.Instance.NextDouble() < 0.45)
								this.Target.World.SpawnGold(this.Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.06)
								this.Target.World.SpawnRandomCraftItem(this.Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.04)
								this.Target.World.SpawnRandomGem(this.Target, plr);
							if (FastRandom.Instance.NextDouble() < 0.15)
								this.Target.World.SpawnRandomPotion(this.Target, plr);
							if (this.Target.World.Game.Difficulty > 1)
								if (FastRandom.Instance.NextDouble() < 0.15)
									this.Target.World.SpawnItem(this.Target, plr, 2087837753);
							if (FastRandom.Instance.NextDouble() < 0.04)
								this.Target.World.SpawnRandomGem(this.Target, plr);
							//Logger.Debug("seed: {0}", seed);
							var dropRates = this.Target.World.Game.IsSeasoned ? LootManager.GetSeasonalDropRates((int)this.Target.Quality, this.Target.Attributes[GameAttribute.Level]) : LootManager.GetDropRates((int)this.Target.Quality, this.Target.Attributes[GameAttribute.Level]);

							float seed = (float)FastRandom.Instance.NextDouble();
							foreach (float rate in dropRates)
								if (seed < (rate * (1f + plr.Attributes[GameAttribute.Magic_Find]) * DiIiS_NA.GameServer.Config.Instance.RateDrop))
								{
									//Logger.Debug("rate: {0}", rate);
									var lootQuality = this.Target.World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)this.Target.Quality, this.Target.World.Game.Difficulty) : LootManager.GetLootQuality((int)this.Target.Quality, this.Target.World.Game.Difficulty);
									this.Target.World.SpawnRandomEquip(this.Target, plr, lootQuality);
									if (this.Target is Goblin)
										this.Target.World.SpawnRandomGem(this.Target, plr);
								}
								else
									break;
							if ((int)this.Target.Quality >= 4 && plr.AdditionalLootItems > 0)
								for (int d = 0; d < plr.AdditionalLootItems; d++)
								{
									var lootQuality = this.Target.World.Game.IsHardcore ? LootManager.GetSeasonalLootQuality((int)this.Target.Quality, this.Target.World.Game.Difficulty) : LootManager.GetLootQuality((int)this.Target.Quality, this.Target.World.Game.Difficulty);
									this.Target.World.SpawnRandomEquip(this.Target, plr, lootQuality);
								}

							if (this.Target is Champion || this.Target is Rare || this.Target is Unique || this.Target is Boss)
							{
								//if (FastRandom.Instance.NextDouble() < LootManager.GetEssenceDropChance(this.Target.World.Game.Difficulty))
								//	this.Target.World.SpawnEssence(this.Target, plr);
								if (this.Target.World.Game.IsSeasoned)
									this.Target.World.SpawnBloodShards(this.Target, plr);
							}

							if (Target.World.Game.IsSeasoned)
                            {
								switch(Target.SNO)
                                {
									case ActorSno._despair: //Раканот
										plr.GrantCriteria(74987254022737);
										break;
									case ActorSno._skeletonking: //Король-скиллет
										plr.GrantCriteria(74987252582955);
										break;
									case ActorSno._siegebreakerdemon: //Siegebreaker - Сделай свой выбор
										plr.GrantCriteria(74987246511881);
										break;
									case ActorSno._x1_adria_boss: //Adria - Я становлюсь Звездой
										plr.GrantCriteria(74987252384014);
										break;
								}
                            }

                            if ((int)this.Target.Quality >= 4)
							{
								if (this.Target.SNO == ActorSno._lacunifemale_c_unique) //Chiltara
									if ((float)FastRandom.Instance.NextDouble() < 0.5f)
										this.Target.World.SpawnItem(this.Target, plr, -799974399);
								if (this.Target.SNO == ActorSno._bigred_izual) //Izual
									if ((float)FastRandom.Instance.NextDouble() < 0.2f)
									{
										switch (this.Target.World.Game.Difficulty)
										{
											case 0:
												this.Target.World.SpawnItem(this.Target, plr, -1463195022);
												break;
											case 1:
												this.Target.World.SpawnItem(this.Target, plr, 645585264);
												break;
											case 2:
												this.Target.World.SpawnItem(this.Target, plr, -501637898);
												break;
											case 3:
												this.Target.World.SpawnItem(this.Target, plr, 253048194);
												break;
											default:
												this.Target.World.SpawnItem(this.Target, plr, -1463195022);
												break;
										}
									}

								switch (this.Target.SNO)
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

				if (this.Context.User is Player & this.Target is Monster)
					if (RandomHelper.Next(0, 100) > 40 & (this.Context.User as Player).Toon.Class == ToonClass.Necromancer)
					{
						var Flesh = this.Context.User.World.SpawnMonster(ActorSno._p6_necro_corpse_flesh, PositionOfDeath);
						Flesh.Attributes[GameAttribute.Necromancer_Corpse_Source_Monster_SNO] = (int)this.Target.SNO;
						Flesh.Attributes.BroadcastChangedIfRevealed();
					}
			}
			if (this.Target is Monster)
				(this.Target as Monster).PlayLore();

			bool isCoop = (this.Target.World.Game.Players.Count > 1);
			bool isHardcore = this.Target.World.Game.IsHardcore;
			bool isSeasoned = this.Target.World.Game.IsSeasoned;
			//114917

			if (this.Target.Quality == 7 || this.Target.Quality == 2 || this.Target.Quality == 4)
			{

			}

			if (this.Target is Boss)
				foreach (Player plr in players)
					switch (this.Target.SNO)
					{
						case ActorSno._skeletonking: //Leoric
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987252301189); if (isHardcore) plr.GrantAchievement(74987243307489); else plr.GrantAchievement(74987249381288);
							break;
						case ActorSno._butcher: //Butcher
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (this.Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243307946);
							if (isCoop) plr.GrantAchievement(74987252696819); if (isHardcore) plr.GrantAchievement(74987254551339); else plr.GrantAchievement(74987258164419);
							plr.SetProgress(1, this.Target.World.Game.Difficulty);
							break;
						case ActorSno._maghda: //Maghda
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987255855515); if (isHardcore) plr.GrantAchievement(74987243307507); else plr.GrantAchievement(74987246434969);
							break;
						case ActorSno._zoltunkulle: //Zoltun Kulle
							if (isCoop) plr.GrantAchievement(74987246137208); if (isHardcore) plr.GrantAchievement(74987243307509); else plr.GrantAchievement(74987252195665);
							break;
						case ActorSno._belial: //Belial (big)
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (this.Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243310916);
							if (isCoop) plr.GrantAchievement(74987256826382); if (isHardcore) plr.GrantAchievement(74987244906887); else plr.GrantAchievement(74987244645044);
							plr.SetProgress(2, this.Target.World.Game.Difficulty);
							break;
						case ActorSno._gluttony: //Gluttony
							if (isCoop) plr.GrantAchievement(74987249112946); if (isHardcore) plr.GrantAchievement(74987243307519); else plr.GrantAchievement(74987259418615);
							break;
						case ActorSno._siegebreakerdemon: //Siegebreaker
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987253664242); if (isHardcore) plr.GrantAchievement(74987243307521); else plr.GrantAchievement(74987248255991);
							break;
						case ActorSno._mistressofpain: //Cydaea
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987257890442); if (isHardcore) plr.GrantAchievement(74987243307523); else plr.GrantAchievement(74987254675042);
							break;
						case ActorSno._azmodan: //Azmodan
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (this.Context.PowerSNO == 71548) //spectral blade
								plr.GrantCriteria(74987243310915);
							if (isCoop) plr.GrantAchievement(74987247100576); if (isHardcore) plr.GrantAchievement(74987251893684); else plr.GrantAchievement(74987247855713);
							plr.SetProgress(3, this.Target.World.Game.Difficulty);
							break;
						case ActorSno._terrordemon_a_unique_1000monster: //Iskatu
							if (isCoop) plr.GrantAchievement(74987255392558); if (isHardcore) plr.GrantAchievement(74987248632930); else plr.GrantAchievement(74987246017001);
							break;
						case ActorSno._despair: //Rakanoth
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987248781143); if (isHardcore) plr.GrantAchievement(74987243307533); else plr.GrantAchievement(74987256508058);
							break;
						case ActorSno._bigred_izual: //Izual
							if (isCoop) plr.GrantAchievement(74987254969009); if (isHardcore) plr.GrantAchievement(74987247989681); else plr.GrantAchievement(74987244988685);
							if (isSeasoned) plr.GrantCriteria(74987249642121);
							break;
						case ActorSno._diablo: //Diablo
							if (this.Context.PowerSNO == 93885) //weapon throw
								plr.GrantAchievement(74987243307050);
							if (isCoop) plr.GrantAchievement(74987250386944); if (isHardcore) plr.GrantAchievement(74987250070969); else plr.GrantAchievement(74987248188984);
							plr.SetProgress(4, this.Target.World.Game.Difficulty);
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
			Player player = (Player)this.Target;
			if (player.Attributes[GameAttribute.Item_Power_Passive, 248629] == 1)
				player.PlayEffectGroup(248680);
			player.StopCasting();
			this.Target.World.BuffManager.RemoveAllBuffs(this.Target, false);
			this.Target.World.PowerManager.CancelAllPowers(this.Target);

			//player.Dead = true;
			player.InGameClient.SendMessage(new VictimMessage()
			{
				PlayerVictimIndex = player.PlayerIndex, //player victim
				KillerLevel = 100,
				KillerPlayerIndex = (this.Context.User is Player ? (this.Context.User as Player).PlayerIndex : -1),                                                                            //player killer(?)
				KillerMonsterRarity = (this.Context.User is Player ? 0 : (int)this.Context.User.Quality),            //quality of actorKiller
				snoKillerActor = this.Context.User is Player ? -1 : (int)this.Context.User.SNO,    //if player killer, then minion SnoId
				KillerTeam = -1,                                                                            //player killer(?)
				KillerRareNameGBIDs = new int[] { -1, -1 },
				snoPowerDmgSource = -1
			});

			//player.PlayAnimation(11, this.Target.AnimationSet.TagMapAnimDefault[AnimationSetKeys.DeathDefault], 1f);
			if (!player.World.Game.PvP)
			{
				this.Target.World.BroadcastIfRevealed(plr => new SetIdleAnimationMessage
				{
					ActorID = this.Target.DynamicID(plr),
					AnimationSNO = AnimationSetKeys.DeadDefault.ID
				}, this.Target);

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
				var tomb = new Headstone(this.Target.World, ActorSno._playerheadstone, new TagMap(), player.PlayerIndex);
				tomb.EnterWorld(player.Position);

				player.Inventory.DecreaseDurability(0.1f);
				if (player.World.Game.IsHardcore)
				{
					player.AddTimedAction(3f, new Action<int>((q) => player.Revive(player.CheckPointPosition)));
					var toon = player.Toon.DBToon;
					toon.Deaths++;
					player.World.Game.GameDBSession.SessionUpdate(toon);
				}
			}
			//}
		}

		private int _FindBestDeathAnimationSNO()
		{
			if (this.Context != null)
			{
				// check if power has special death animation, and roll chance to use it
				TagKeyInt specialDeathTag = _GetTagForSpecialDeath(this.Context.EvalTag(PowerKeys.SpecialDeathType));
				if (specialDeathTag != null)
				{
					float specialDeathChance = this.Context.EvalTag(PowerKeys.SpecialDeathChance);
					if (PowerContext.Rand.NextDouble() < specialDeathChance)
					{
						int specialSNO = _GetSNOFromTag(specialDeathTag);
						if (specialSNO != -1)
						{
							return specialSNO;
						}
					}
					// decided not to use special death or actor doesn't have it, just fall back to normal death anis
				}

				int sno = _GetSNOFromTag(this.DeathDamageType.DeathAnimationTag);
				if (sno != -1)
					return sno;

				//if (this.Target.ActorSNO.Name.Contains("Spiderling")) return _GetSNOFromTag(new TagKeyInt(69764));

				//Logger.Debug("monster animations:");
				//foreach (var anim in this.Target.AnimationSet.TagMapAnimDefault)
				//	Logger.Debug("animation: {0}", anim.ToString());

				// load default ani if all else fails
				return _GetSNOFromTag(AnimationSetKeys.DeathDefault);
			}
			else
				return -1;
		}

		private int _GetSNOFromTag(TagKeyInt tag)
		{
			if (this.Target.AnimationSet != null && this.Target.AnimationSet.TagMapAnimDefault.ContainsKey(tag))
				return this.Target.AnimationSet.TagMapAnimDefault[tag];
			else
				return -1;
		}

		private static TagKeyInt _GetTagForSpecialDeath(int specialDeathType)
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

//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Minions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;
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

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	//Complete
	#region SpectralBlade
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Signature.SpectralBlade)]
	public class WizardSpectralBlade : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(208493)) //Prodigy (wizard)
				GeneratePrimaryResource(5f);

			if (HasBuff<EnergyTwister.TwisterBuff>(User))   //EnergyTwister -> Storm Chaser
			{
				var mult = User.World.BuffManager.GetFirstBuff<EnergyTwister.TwisterBuff>(User).StackCount;
				var proj = new Projectile(this, 210896, User.Position);
				proj.Position.Z += 5f;
				proj.RadiusMod = 1.5f;
				proj.OnCollision = hit =>
				{
					if (hit == null) return;
					WeaponDamage(hit, 1.96f * mult, DamageType.Lightning);
				};
				proj.Launch(TargetPosition, 0.8f);
				RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.EnergyTwister);
			}

			User.PlayEffectGroup(RuneSelect(321961, 322243, 323989, 322587, 324806, 323162));

			if ((User as Player).SkillSet.HasPassive(208823) && Rand.NextDouble() < GetProcCoefficient()) //ArcaneDynamo (wizard)
				AddBuff(User, new DynamoBuff());

			var dmgType = DamageType.Arcane;
			if (Rune_A > 0) dmgType = DamageType.Fire;
			if (Rune_C > 0) dmgType = DamageType.Cold;

			//Rune B is in SF3
			AttackPayload blade = new AttackPayload(this);
			blade.Targets = GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(3), 120f);
			blade.AddWeaponDamage(Rune_B > 0 ? 2.35f : 1.71f, dmgType);
			blade.OnHit = hitPayload =>
			{
				if (Rune_A > 0)         //Flame Blades, done in HitPayload
					AddBuff(User, new FlameBuff());     //buff slot 4

				if (Rune_C > 0)         //Ice Blades, done in AttackPayload
				{
					if (HasBuff<DebuffFrozen>(hitPayload.Target))
					{
						if (!HasBuff<BladesChcDebuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new BladesChcDebuff(ScriptFormula(23), WaitSeconds(3f)));
					}
					else
						if (HasBuff<DebuffChilled>(hitPayload.Target) && Rand.NextDouble() < ScriptFormula(17))
						AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(3f)));
				}

				if (Rune_D > 0)         //Siphoning Blade
					if (User is Player)
						(User as Player).GeneratePrimaryResource(ScriptFormula(13));

				if (Rune_E > 0 && !HasBuff<BarrierBuff>(User))          //Barrier Blades, buff slot 1
					AddBuff(User, new BarrierBuff(User.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.04f, WaitSeconds(ScriptFormula(9))));
			};
			blade.Apply();
			yield break;
		}

		[ImplementsPowerBuff(4, true)]
		public class FlameBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(6));
				MaxStackCount = 30;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//_AddAmp();	
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
			private void _AddAmp()
			{
			}
		}

		[ImplementsPowerBuff(3)]
		public class BladesChcDebuff : PowerBuff
		{
			public float Percentage = 0;
			public BladesChcDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(1)]
		public class BarrierBuff : PowerBuff
		{
			public float HPTreshold = 0;
			public BarrierBuff(float hpTreshold, TickTimer timeout)
			{
				HPTreshold = hpTreshold;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (HPTreshold > 0)
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					{
						float dmg = (payload as HitPayload).TotalDamage;
						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= dmg;
						if (HPTreshold < 0)
							User.World.BuffManager.RemoveBuff(User, this);
					}
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region Meteor
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.Meteor)]
	public class WizardMeteor : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			//Rune_D here as well.
			UsePrimaryResource(ScriptFormula(8));
			if (Rune_A > 0) StartCooldown(15f);
			if (Rune_B > 0) StartCooldown(1f);      //Meteor shower anti-spam

			// cast effect
			User.PlayEffectGroup(RuneSelect(71141, 71141, 71141, 92222, 217377, 217461));

			// HACK: mooege's 100ms update rate is a little to slow for the impact to appear right on time so
			// an 100ms is shaved off the wait time
			TickTimer waitForImpact = WaitSeconds(ScriptFormula(4) - 0.1f);

			List<Vector3D> impactPositions = new List<Vector3D>();
			int meteorCount = Rune_B > 0 ? (int)ScriptFormula(9) : 1;

			// pending effect + meteor
			for (int n = 0; n < meteorCount; ++n)
			{
				Vector3D impactPos;
				if (meteorCount > 1)
					impactPos = new Vector3D(TargetPosition.X + ((float)Rand.NextDouble() - 0.5f) * 25,
											 TargetPosition.Y + ((float)Rand.NextDouble() - 0.5f) * 25,
											 TargetPosition.Z);
				else
					impactPos = TargetPosition;

				SpawnEffect(RuneSelect(86790, 215853, 91440, 92030, 217142, 217457), impactPos, 0, WaitSeconds(5f));
				impactPositions.Add(impactPos);

				if (meteorCount > 1)
					yield return WaitSeconds(0.1f);
			}

			// wait for meteor impact(s)
			yield return waitForImpact;

			// impact effects
			foreach (var impactPos in impactPositions)
			{
				// impact
				TickTimer poolTime = null;
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(impactPos, ScriptFormula(3));
				attack.AddWeaponDamage(ScriptFormula(0), RuneSelect(DamageType.Fire, DamageType.Fire, DamageType.Fire, DamageType.Cold, DamageType.Arcane, DamageType.Lightning));
				attack.OnHit = hitPayload =>
				{
					if (Rune_C > 0)     //Comet
						if (!HasBuff<DebuffFrozen>(hitPayload.Target) && Rand.NextDouble() < ScriptFormula(11))
							AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(19))));

					if (Rune_E > 0)     //Lightning Bind
					{
						if (hitPayload.IsCriticalHit)
						{
							poolTime = WaitSeconds(ScriptFormula(7));
							if (!HasBuff<DebuffRooted>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffRooted(WaitSeconds(ScriptFormula(26))));
						}
					}
					else
					{
						poolTime = WaitSeconds(ScriptFormula(5));
					}
				};
				attack.Apply();

				if (!(Rune_B > 0))
				{
					var moltenFire = SpawnEffect(RuneSelect(86769, 215809, 91441, 92031, 217139, 217458), impactPos, 0, poolTime);
					moltenFire.UpdateDelay = 1f;
					moltenFire.OnUpdate = () =>
					{
						AttackPayload DOTattack = new AttackPayload(this);
						DOTattack.Targets = GetEnemiesInRadius(impactPos, ScriptFormula(3));
						DOTattack.AddWeaponDamage(ScriptFormula(2), RuneSelect(DamageType.Fire, DamageType.Fire, DamageType.Fire, DamageType.Cold, DamageType.Arcane, DamageType.Lightning));
						DOTattack.OnHit = hitPayload =>
						{
							if (Rune_C > 0)     //Comet
								if (!HasBuff<DebuffChilled>(hitPayload.Target))
									AddBuff(hitPayload.Target, new DebuffChilled(ScriptFormula(6), WaitSeconds(ScriptFormula(5))));
						};
						DOTattack.Apply();
					};
				}

				// pool effect
				if (!(Rune_B > 0))
				{
					SpawnEffect(RuneSelect(90364, 90364, -1, 92032, 217307, 217459), impactPos, 0,
						WaitSeconds(ScriptFormula(5)));
				}

				if (meteorCount > 1)
					yield return WaitSeconds(0.1f);
			}
		}
	}

	#endregion

	//Complete
	#region Electrocute
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Signature.Electrocute)]
	public class WizardElectrocute : ChanneledSkill
	{
		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.5f;
		}

		public override IEnumerable<TickTimer> Main()
		{
			User.TranslateFacing(TargetPosition);

			if (HasBuff<EnergyTwister.TwisterBuff>(User))   //EnergyTwister -> Storm Chaser
			{
				var mult = User.World.BuffManager.GetFirstBuff<EnergyTwister.TwisterBuff>(User).StackCount;
				var proj = new Projectile(this, 210896, User.Position);
				proj.Position.Z += 5f;
				proj.RadiusMod = 1.5f;
				proj.OnCollision = hit =>
				{
					if (hit == null) return;
					WeaponDamage(hit, 1.96f * mult, DamageType.Lightning);
				};
				proj.Launch(TargetPosition, 0.8f);
				RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.EnergyTwister);
			}

			if ((User as Player).SkillSet.HasPassive(208493))  //Prodigy (wizard)
				GeneratePrimaryResource(5f);

			if ((User as Player).SkillSet.HasPassive(208823) && Rand.NextDouble() < GetProcCoefficient()) //ArcaneDynamo (wizard)
				AddBuff(User, new DynamoBuff());

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 35f));
			if (Rune_A > 0)         //Lightning Blast
			{
				var proj = new Projectile(this, 76019, User.Position);
				proj.Position.Z += 5f;  // fix height
				proj.RadiusMod = 5f;
				proj.OnCollision = (hit) =>
				{
					hit.PlayEffectGroup(77858);
					WeaponDamage(hit, ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Lightning);
				};
				proj.Launch(TargetPosition, 1.25f);
			}
			else if (Rune_C > 0)        //Arc Lightning
			{
				User.PlayEffectGroup(77807);
				WeaponDamage(GetEnemiesInArcDirection(User.Position, TargetPosition, ScriptFormula(2), 120f), ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Lightning);
			}
			else if (Target == null)
			{
				User.AddRopeEffect(30913, TargetPosition);
			}
			else
			{
				IList<Actor> targets = new List<Actor>() { Target };
				Actor ropeSource = User;
				Actor curTarget = Target;
				while (targets.Count < ScriptFormula(9) + 1) // original target + bounce 2 times
				{
					// replace source with proxy if it died while doing bounce delay
					if (ropeSource.World == null)
						ropeSource = SpawnProxy(ropeSource.Position);

					if (curTarget.World != null)
					{
						ropeSource.AddRopeEffect(0x78c0, curTarget);
						ropeSource = curTarget;

						AttackPayload attack = new AttackPayload(this);
						attack.AddWeaponDamage(ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Lightning);
						attack.SetSingleTarget(curTarget);
						attack.OnHit = hitPayload =>
						{
							if (Rune_E > 0 && hitPayload.IsCriticalHit)         //Forked Lightning
							{
								Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(hitPayload.Target.Position, hitPayload.Target.Position + new Vector3D(10f, 0, 0), 90f, 4);

								foreach (Vector3D missilePos in projDestinations)
								{
									var proj = new Projectile(this, 176247, Target.Position);
									proj.Position.Z += 5f;
									proj.OnCollision = (hit) =>
									{
										if (hit == hitPayload.Target) return;
										SpawnEffect(176262, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
										WeaponDamage(hit, ScriptFormula(12), DamageType.Lightning);
										proj.Destroy();
									};
									proj.Launch(missilePos, 1.25f);
								}
							}
						};
						attack.Apply();

						if (Rune_D > 0) GeneratePrimaryResource(ScriptFormula(6));          //Surge of Power
					}
					else
					{
						// early out if monster to be bounced died prematurely
						break;
					}

					curTarget = GetEnemiesInRadius(curTarget.Position, ScriptFormula(2), (int)ScriptFormula(9)).Actors.FirstOrDefault(t => !targets.Contains(t));
					if (curTarget != null)
					{
						targets.Add(curTarget);
						yield return WaitSeconds(0.150f);
					}
					else
					{
						break;
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region MagicMissile
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Signature.MagicMissile)]
	public class WizardMagicMissile : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(208493)) //Prodigy (wizard)
				GeneratePrimaryResource(5f);

			if (HasBuff<EnergyTwister.TwisterBuff>(User))   //EnergyTwister -> Storm Chaser
			{
				var mult = User.World.BuffManager.GetFirstBuff<EnergyTwister.TwisterBuff>(User).StackCount;
				var proj = new Projectile(this, 210896, User.Position);
				proj.Position.Z += 5f;
				proj.RadiusMod = 1.5f;
				proj.OnCollision = hit =>
				{
					if (hit == null) return;
					WeaponDamage(hit, 1.96f * mult, DamageType.Lightning);
				};
				proj.Launch(TargetPosition, 0.8f);
				RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.EnergyTwister);
			}

			if ((User as Player).SkillSet.HasPassive(208823) && Rand.NextDouble() < GetProcCoefficient()) //ArcaneDynamo (wizard)
				AddBuff(User, new DynamoBuff());

			if (Rune_B > 0)     //Split
			{
				Vector3D[] projDestinations = PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, ScriptFormula(8) / 5f, (int)ScriptFormula(5));

				foreach (var position in projDestinations)
				{
					var proj = new Projectile(this, 99567, User.Position);
					proj.OnCollision = (hit) =>
					{
						SpawnEffect(99572, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
						WeaponDamage(hit, ScriptFormula(1), DamageType.Arcane);
						proj.Destroy();
					};
					proj.Launch(position, ScriptFormula(4));
				}
				yield break;
			}

			if (Rune_E > 0)     //Seeker
			{
				var projectile = new Projectile(this, 99567, User.Position);
				var target = GetEnemiesInArcDirection(User.Position, TargetPosition, 60f, 60f).GetClosestTo(User.Position);

				if (target != null)
				{
					projectile.Launch(target.Position, ScriptFormula(4));
					projectile.OnCollision = (hit) =>
					{
						SpawnEffect(99572, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
						projectile.Destroy();
						WeaponDamage(hit, ScriptFormula(1), DamageType.Arcane);
					};
				}
				else
				{
					projectile.Launch(TargetPosition, ScriptFormula(4));
					projectile.OnCollision = (hit) =>
					{
						SpawnEffect(99572, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
						projectile.Destroy();
						WeaponDamage(hit, ScriptFormula(1), DamageType.Arcane);
					};

					for (int i = 0; i < 2; i++)     //yuck... need proper guided projectile
					{
						target = GetEnemiesInArcDirection(User.Position, TargetPosition, 60f, 60f).GetClosestTo(User.Position);

						if (target != null)
						{
							var projectileSeek = new Projectile(this, 99567, projectile.Position);
							projectile.Destroy();
							projectileSeek.Launch(target.Position, ScriptFormula(4));
							projectileSeek.OnCollision = (hit) =>
							{
								SpawnEffect(99572, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)
								projectileSeek.Destroy();
								WeaponDamage(hit, ScriptFormula(1), DamageType.Arcane);
							};
							i = 1;
						}
						else yield return WaitTicks(6);
					}
				}
				yield break;
			}

			//base effect
			var arrow = new Projectile(this, RuneSelect(99567, 99629, 99567, 189372, 189373, 99567), User.Position);
			arrow.OnCollision = hit =>
			{
				//SpawnEffect(99572, arrow.Position);
				if (Rune_C > 0)     //Conflagrate
				{
					SpawnEffect(189460, hit.Position + new Vector3D(0, 0, 5f));
					AddBuff(hit, new MissleFireDoTBuff());
					return;
				}

				if (Rune_D > 0)     //Glacial Spike
				{
					//SpawnEffect(328161, hit.Position);
					SpawnEffect(328146, hit.Position);
					AttackPayload blast = new AttackPayload(this);
					blast.Targets = GetEnemiesInRadius(hit.Position, ScriptFormula(17));
					blast.AddWeaponDamage(ScriptFormula(2), DamageType.Cold);
					blast.OnHit = hitPayload =>
					{
						if (!HasBuff<DebuffFrozen>(hitPayload.Target))
							if (!HasBuff<FreezeImmuneBuff>(hitPayload.Target))
							{
								AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(19))));
								AddBuff(hitPayload.Target, new FreezeImmuneBuff(WaitSeconds(ScriptFormula(23))));
							}
					};
					blast.Apply();
					arrow.Destroy();
					return;
				}

				SpawnEffect(99572, hit.Position + new Vector3D(0, 0, 5f));
				WeaponDamage(hit, Rune_A > 0 ? ScriptFormula(1) : ScriptFormula(14), DamageType.Arcane);

				arrow.Destroy();
			};
			arrow.Launch(TargetPosition, ScriptFormula(4));

			yield break;
		}

		[ImplementsPowerBuff(3)]
		public class FreezeImmuneBuff : PowerBuff
		{
			public FreezeImmuneBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(0, true)]
		class MissleFireDoTBuff : PowerBuff
		{
			public float dps;
			const float DotRate = 1f;
			TickTimer DotTimer = null;
			public override void Init()
			{
				dps = ScriptFormula(15);
				Timeout = WaitSeconds(ScriptFormula(9));
				MaxStackCount = 10;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DotTimer == null)
					DotTimer = WaitSeconds(DotRate);

				if (DotTimer.TimedOut)
				{
					DotTimer = WaitSeconds(DotRate);
					WeaponDamage(Target, dps * StackCount, DamageType.Fire);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region Hydra
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.Hydra)]
	public class WizardHydra : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.Hydra);
			yield return WaitSeconds(0.4f);
			AddBuff(User, new HydraBuff(TargetPosition, WaitSeconds(ScriptFormula(0))));

			yield break;
		}

		[ImplementsPowerBuff(3)]
		public class HydraBuff : PowerBuff
		{
			TickTimer DotTimer = null;
			Vector3D TgtPosition = null;
			private float FireRate = 1f;
			private EffectActor[] Hydras = new EffectActor[3] { null, null, null };
			private EffectActor[] LavaPools = new EffectActor[3] { null, null, null };
			private EffectActor[] Proxies = new EffectActor[3] { null, null, null };
			public HydraBuff(Vector3D targetPosition, TickTimer timeout)
			{
				TgtPosition = targetPosition;
				Timeout = timeout;
			}

			public override void Init()     //Hydra APS should scale with wizard APS
			{
				if (User.Attributes[GameAttribute.Attacks_Per_Second_Total] > 1f)
					FireRate /= User.Attributes[GameAttribute.Attacks_Per_Second_Total];
				if (FireRate < 0.25f) FireRate = 0.25f;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_D > 0)         //Mammoth Hydra has only 1 head
				{
					if (User.World.CheckLocationForFlag(TgtPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
					{
						Hydras[0] = new EffectActor(this, 83959, TgtPosition);
						Hydras[0].Timeout = Timeout;
						Hydras[0].Scale = 1.9f;
						Hydras[0].Spawn();

						LavaPools[0] = new EffectActor(this, 326277, TgtPosition);
						LavaPools[0].Timeout = Timeout;
						LavaPools[0].Scale = 1.9f;
						LavaPools[0].Spawn();

						LavaPools[0].PlayEffectGroup(83959);
					}
					return true;
				}

				Vector3D[] spawnPoints = PowerMath.GenerateSpreadPositions(TgtPosition, TgtPosition + new Vector3D(3f, 0, 0), 120, 3);

				for (int i = 0; i < spawnPoints.Count(); i++)
				{
					if (!User.World.CheckLocationForFlag(spawnPoints[i], DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
						continue;
					LavaPools[i] = SpawnEffect(RuneSelect(81103, 83028, 81238, 326285, 83964, 81239), spawnPoints[i], 0, Timeout); //Lava Pool Spawn
					LavaPools[i].PlayEffectGroup(RuneSelect(81102, 82995, 82116, 86328, 326277, 81301));

					Hydras[i] = SpawnEffect(RuneSelect(80745, 82972, 82109, 325807, 83959, 81515), spawnPoints[i], 0, Timeout);

					if (Rune_B > 0)
					{
						Proxies[i] = SpawnProxy(spawnPoints[i], Timeout);
						Proxies[i].Position.Z += 3f;
					}
				}

				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DotTimer == null || DotTimer.TimedOut)
				{
					DotTimer = WaitSeconds(FireRate);
					int i = 0;

					foreach (var hydra in Hydras)
					{
						if (hydra == null) continue;
						var target = GetEnemiesInRadius(hydra.Position, 40f).GetClosestTo(hydra.Position);
						if (target == null) continue;
						hydra.TranslateFacing(target.Position, true);

						if (Rune_A > 0)     //Frost Hydra
						{
							float castAngle = MovementHelpers.GetFacingAngle(hydra.Position, target.Position);
							hydra.PlayEffectGroup(84033);
							WeaponDamage(GetEnemiesInArcDirection(hydra.Position, target.Position, 20f, 120f), ScriptFormula(20), DamageType.Cold);
							continue;
						}
						if (Rune_B > 0)     //Lightning Hydra
						{
							if (Proxies[i] != null) Proxies[i].AddRopeEffect(83875, target);
							i++;
							WeaponDamage(target, ScriptFormula(20), DamageType.Lightning);
							if (target.World == null) continue;

							IList<Actor> targets = new List<Actor>() { target };
							Actor curTarget = target;
							Actor nextTarget = null;
							var c = 0;
							while (targets.Count() < 3)
							{
								nextTarget = GetEnemiesInRadius(curTarget.Position, 6f).Actors.FirstOrDefault(a => !targets.Contains(a));
								if (nextTarget == null) break;

								curTarget.AddRopeEffect(83875, nextTarget);
								WeaponDamage(nextTarget, ScriptFormula(20), DamageType.Lightning);
								curTarget = nextTarget;
								targets.Add(curTarget);

								c++;
								if (c > 6) break;
							}
							continue;
						}
						if (Rune_D > 0)     //Mammoth Hydra
						{
							float castAngle = MovementHelpers.GetFacingAngle(hydra.Position, target.Position);
							var firewall = SpawnEffect(86082, hydra.Position, castAngle, WaitSeconds(FireRate));
							firewall.UpdateDelay = FireRate;
							firewall.OnUpdate = () =>
							{
								WeaponDamage(GetEnemiesInBeamDirection(hydra.Position, target.Position, 50f, 5f), ScriptFormula(20) * 1.5f, DamageType.Fire);
							};
							continue;
						}

						var proj = new Projectile(this, RuneSelect(77116, 83043, -1, 77116, -1, 77097), hydra.Position);
						proj.Position.Z += 5f;  // fix height
						proj.OnCollision = (hit) =>
						{
							if (Rune_C > 0)     //Blazing Hydra
							{
								SpawnEffect(366983, hit.Position);
								AttackPayload explosion = new AttackPayload(this);
								explosion.Targets = GetEnemiesInRadius(hit.Position, 10f);
								explosion.OnHit = hitPayload =>
								{
									AddBuff(hitPayload.Target, new HydraFireDoTBuff());
								};
								explosion.Apply();
								proj.Destroy();
								return;
							}

							else if (Rune_E > 0)    //Arcane Hydra
							{
								hit.PlayEffectGroup(81874);
								WeaponDamage(GetEnemiesInRadius(hit.Position, 10f), ScriptFormula(20), DamageType.Arcane);
								proj.Destroy();
								return;
							}

							hit.PlayEffectGroup(RuneSelect(219760, 219770, 219776, 219789, -1, 81739));
							WeaponDamage(hit, ScriptFormula(21), DamageType.Fire);

							proj.Destroy();
						};
						proj.Launch(target.Position, 1f);
					}
					i = 0;
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();

				for (int i = 0; i < 3; i++)
				{
					if (Hydras[i] != null) Hydras[i].Destroy();
					if (LavaPools[i] != null) LavaPools[i].Destroy();
					if (Proxies[i] != null) Proxies[i].Destroy();
				}
			}
		}

		[ImplementsPowerBuff(4, true)]
		class HydraFireDoTBuff : PowerBuff
		{
			public float dps;
			const float DotRate = 1f;
			TickTimer DotTimer = null;
			public override void Init()
			{
				dps = ScriptFormula(20);
				Timeout = WaitSeconds(ScriptFormula(24));
				MaxStackCount = 15;         //retail says stacking infinitely
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DotTimer == null)
					DotTimer = WaitSeconds(DotRate);

				if (DotTimer.TimedOut)
				{
					DotTimer = WaitSeconds(DotRate);
					WeaponDamage(Target, dps * StackCount, DamageType.Fire);
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region ArcaneOrb
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.ArcaneOrb)]
	public class ArcaneOrb : Skill
	{
		TickTimer OrbTimer = null;
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			if (Rune_C > 0)     //Arcane Orbit
			{
				User.World.BuffManager.RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.ArcaneOrb);
				AddBuff(User, new Orbit4());
				yield break;
			}

			var startPosition = User.Position;
			var maxRange = 60f;
			var multiplier = ScriptFormula(7);
			var dmgType = DamageType.Arcane;

			if (Rune_A > 0) multiplier = ScriptFormula(3);  //Obliterate
			if (Rune_B > 0)             //Spark
			{
				multiplier = ScriptFormula(26);
				dmgType = DamageType.Lightning;
			}
			if (Rune_D > 0)             //Scorch
			{
				maxRange = 40f;
				multiplier = ScriptFormula(40);
				dmgType = DamageType.Fire;
			}
			if (Rune_E > 0)             //Frozen Orb
			{
				maxRange = 40f;
				multiplier = ScriptFormula(8);
				dmgType = DamageType.Cold;
			}

			if (Rune_B > 0 || Rune_D > 0 || Rune_E > 0)
			{
				var proj2 = new Projectile(this, RuneSelect(6515, 130073, 317809, -1, 216040, 317398), startPosition);
				if (Rune_B <= 0) proj2.Position.Z += 5f;
				proj2.OnUpdate = () =>      //Resolved all SF to optimize this Update
				{
					if (Rune_B <= 0)
					{
						if (PowerMath.Distance2D(proj2.Position, startPosition + new Vector3D(0, 0, 5f)) > maxRange)
						{
							proj2.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
							WeaponDamage(GetEnemiesInRadius(proj2.Position, 15f), multiplier, dmgType);
							proj2.Destroy();
							return;
						}
						if (PowerMath.Distance2D(proj2.Position, TargetPosition + new Vector3D(0, 0, 5f)) < 3f)
						{
							proj2.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
							WeaponDamage(GetEnemiesInRadius(proj2.Position, 15f), multiplier, dmgType);
							proj2.Destroy();
							return;
						}
					}

					if (OrbTimer == null || OrbTimer.TimedOut)
					{
						OrbTimer = WaitSeconds(0.8f);

						var targets = GetEnemiesInRadius(proj2.Position, Rune_D > 0 ? 10f : 15f).Actors;
						foreach (var target in targets)
						{
							if (Rune_B > 0)     //Spark, done in HitPayload
							{
								proj2.AddRopeEffect(0x78c0, target);
								AddBuff(User, new OrbShockBuff());      //buff slot 6
							}
							WeaponDamage(target, multiplier, dmgType);
						}

						if (Rune_D > 0)     //Scorch
						{
							var firePool = SpawnEffect(339443, proj2.Position, 0, WaitSeconds(5f));
							firePool.UpdateDelay = 1f;
							firePool.OnUpdate = () =>
							{
								var poolTargets = GetEnemiesInRadius(firePool.Position, 5f).Actors;
								foreach (var target in poolTargets)
									if (!HasBuff<PoolDmgBuff>(target))
									{
										WeaponDamage(target, 1.468f, dmgType);
										AddBuff(target, new PoolDmgBuff(WaitSeconds(0.8f)));
									}
							};
						}
					}
				};
				if (Rune_B > 0)
				{
					proj2.OnArrival = () =>
					{
						proj2.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
						WeaponDamage(GetEnemiesInRadius(proj2.Position, 15f), multiplier, dmgType);
						proj2.Destroy();
					};
					TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 30f));
					proj2.LaunchArc(TargetPosition, 9f, -0.07f);
				}
				else proj2.Launch(TargetPosition, 0.5f);

				yield break;
			}

			//base and Rune A effects
			var proj = new Projectile(this, RuneSelect(6515, 130073, 317652, -1, 216040, 317398), startPosition);
			proj.Position.Z += 5f;
			proj.OnUpdate = () =>
			{
				if (PowerMath.Distance2D(proj.Position, startPosition + new Vector3D(0, 0, 5f)) > maxRange)
				{
					proj.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
					WeaponDamage(GetEnemiesInRadius(proj.Position, Rune_A > 0 ? 8f : 15f), multiplier, dmgType);
					proj.Destroy();
					return;
				}
				if (PowerMath.Distance2D(proj.Position, TargetPosition + new Vector3D(0, 0, 5f)) < 3f)
				{
					proj.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
					WeaponDamage(GetEnemiesInRadius(proj.Position, Rune_A > 0 ? 8f : 15f), multiplier, dmgType);
					proj.Destroy();
				}
			};
			proj.OnCollision = hit =>
			{
				hit.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, 317503));
				WeaponDamage(GetEnemiesInRadius(hit.Position, Rune_A > 0 ? 8f : 15f), multiplier, dmgType);
				proj.Destroy();
			};
			proj.Launch(TargetPosition, Rune_A > 0 ? 1.0f : 0.5f);
			yield break;
		}

		[ImplementsPowerBuff(6, true)]
		public class OrbShockBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(2f);
				MaxStackCount = 15;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//_AddAmp();	
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
			private void _AddAmp()
			{
			}
		}

		[ImplementsPowerBuff(4)]
		class PoolDmgBuff : PowerBuff
		{
			public PoolDmgBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		abstract class OrbitBase : PowerBuff
		{
			TickTimer timer;

			public override void Init()
			{
				timer = WaitSeconds(1f);
			}

			public override bool Update()
			{

				if (base.Update())
					return true;

				if (timer.TimedOut)
				{
					var target = GetEnemiesInRadius(User.Position, 10f).GetClosestTo(User.Position);
					if (target != null)
					{
						WeaponDamage(GetEnemiesInRadius(target.Position, 10f), ScriptFormula(3), DamageType.Arcane);
						OrbitUsed();
						return true;
					}
				}

				return false;
			}

			protected abstract void OrbitUsed();
		}
		[ImplementsPowerBuff(0)]
		class Orbit1 : OrbitBase
		{
			protected override void OrbitUsed()
			{
				// do nothing, orbits all used up
			}
		}
		[ImplementsPowerBuff(1)]
		class Orbit2 : OrbitBase
		{
			protected override void OrbitUsed()
			{
				AddBuff(Target, new Orbit1());
			}
		}
		[ImplementsPowerBuff(2)]
		class Orbit3 : OrbitBase
		{
			protected override void OrbitUsed()
			{
				AddBuff(Target, new Orbit2());
			}
		}
		[ImplementsPowerBuff(3)]
		class Orbit4 : OrbitBase
		{
			protected override void OrbitUsed()
			{
				AddBuff(Target, new Orbit3());
			}
		}
	}
	#endregion

	//Complete	
	#region EnergyTwister
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.EnergyTwister)]
	public class EnergyTwister : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			TickTimer timeout = WaitSeconds(ScriptFormula(8));
			UsePrimaryResource(ScriptFormula(15));

			var multiplier = ScriptFormula(16) * ScriptFormula(12) * 0.5f / ScriptFormula(8);   //damage per update
			var dmgType = DamageType.Arcane;
			if (Rune_A > 0) dmgType = DamageType.Fire;
			if (Rune_C > 0) dmgType = DamageType.Lightning;
			if (Rune_D > 0) dmgType = DamageType.Cold;          //Mistral Breeze

			if (Rune_C > 0)             //Storm Chaser
				AddBuff(User, new TwisterBuff());

			var Twister = new EffectActor(this, RuneSelect(6560, 319692, 6560, 215324, 323092, 210804), (Rune_E > 0 ? TargetPosition : User.Position));
			Twister.Timeout = WaitSeconds(ScriptFormula(8));
			Twister.Scale = 1f;
			if (Twister != null) Twister.Spawn();       //Could be wiped by Rune_B at this point

			Twister.UpdateDelay = 0.5f;
			Twister.OnUpdate = () =>
			{
				if (Twister.World == null) return;

				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(Twister.Position, 6f);
				attack.AddWeaponDamage(multiplier, dmgType);
				attack.OnHit = hitPayload =>
				{
					if (Rune_A > 0)         //Gale Force, done in HitPayload
						if (!HasBuff<GaleForceDebuff>(hitPayload.Target))
							AddBuff(hitPayload.Target, new GaleForceDebuff(ScriptFormula(23), WaitSeconds(ScriptFormula(22))));
				};
				attack.Apply();

				if (Rune_E > 0) return;     //Wicked Wind

				ActorMover _twisterMover = new ActorMover(Twister);

				_twisterMover.Move(TargetPosition, ScriptFormula(18), new ACDTranslateNormalMessage
				{
					SnapFacing = true,
					AnimationTag = 69728,
				});
				TargetPosition = PowerMath.GenerateSpreadPositions(Twister.Position, TargetPosition, 20f, 3)[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 3)];

				if (Rune_B > 0)     //Raging Storm
				{
					var twisters = Twister.GetActorsInRange<EffectActor>(5f).Where(i => ((i.ActorSNO.Id == 6560) && (i != Twister)));
					if (twisters.Count() > 0)
					{
						foreach (var twist in twisters)
							twist.Destroy();

						var bigMultiplier = ScriptFormula(2) * 0.5f / ScriptFormula(4);

						var BigTwister = new EffectActor(this, 77333, Twister.Position);
						BigTwister.Timeout = WaitSeconds(ScriptFormula(4));
						BigTwister.Scale = 1f;
						BigTwister.Spawn();

						BigTwister.UpdateDelay = 0.5f;
						BigTwister.OnUpdate = () =>
						{
							ActorMover _bigTwisterMover = new ActorMover(BigTwister);
							_bigTwisterMover.Move(TargetPosition, ScriptFormula(18), new ACDTranslateNormalMessage
							{
								SnapFacing = true,
								AnimationTag = 69728,
							});
							TargetPosition = PowerMath.GenerateSpreadPositions(BigTwister.Position, TargetPosition, 20f, 3)[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 3)];
							WeaponDamage(GetEnemiesInRadius(BigTwister.Position, 12f), bigMultiplier, DamageType.Arcane);
						};
						Twister.Destroy();
					}
				}
			};

			yield break;
		}

		[ImplementsPowerBuff(2)]
		public class GaleForceDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public GaleForceDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(3, true)]
		public class TwisterBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(10));
				MaxStackCount = (int)ScriptFormula(9);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region Disintegrate
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.Disintegrate)]
	public class WizardDisintegrate : ChanneledSkill
	{
		const float BeamLength = 40f;
		private Actor _target = null;
		private void _calcTargetPosition()
		{
			// project beam end to always be a certain length
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition,
															 new Vector3D(User.Position.X, User.Position.Y, TargetPosition.Z),
															 BeamLength);
		}

		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.3f;

			_calcTargetPosition();
			if (Rune_C <= 0)
			{
				_target = SpawnEffect(RuneSelect(52687, 52687, 93544, -1, 52687, 215723), TargetPosition, 0, WaitInfinite());
				User.AddComplexEffect(RuneSelect(18792, 18792, 93529, -1, 93593, 216368), _target);
			}
		}

		public override void OnChannelClose()
		{
			if (_target != null) _target.Destroy();
		}

		public override void OnChannelUpdated()
		{
			_calcTargetPosition();
			User.TranslateFacing(TargetPosition);
			// client updates target actor position
		}

		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(ScriptFormula(20));

			if (Rune_C > 0)         //Entropy
			{
				AddBuff(User, new FieldBuff());
				WeaponDamage(GetEnemiesInArcDirection(User.Position, TargetPosition, 20f, 120f), ScriptFormula(1) * ScriptFormula(2) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Arcane);
			}
			else
				foreach (Actor actor in GetEnemiesInRadius(User.Position, BeamLength + 10f).Actors)
				{
					if (PowerMath.PointInBeam(actor.Position, User.Position, TargetPosition, (Rune_B > 0 ? 6f : 3f)))
					{
						AttackPayload ray = new AttackPayload(this);
						ray.SetSingleTarget(actor);
						ray.AddWeaponDamage(ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Arcane);
						ray.OnHit = hitPayload =>
						{
							if (Rune_A > 0)     //Intensify, done in HitPayload
								if (!HasBuff<IntensifyDebuff>(hitPayload.Target))
									AddBuff(hitPayload.Target, new IntensifyDebuff(ScriptFormula(13), WaitSeconds(ScriptFormula(14))));
						};
						ray.OnDeath = DeathPayload =>
						{
							if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(12))    //Volatility
							{
								SpawnProxy(DeathPayload.Target.Position).PlayEffectGroup(93574);
								WeaponDamage(GetEnemiesInRadius(DeathPayload.Target.Position, ScriptFormula(25)), ScriptFormula(9), DamageType.Arcane);
							}
						};
						ray.Apply();
					}
				}

			if (Rune_D > 0)         //Chaos Nexus
			{
				User.PlayEffectGroup(95258);
				WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(25)), ScriptFormula(7), DamageType.Arcane);
			}

			yield break;
		}

		[ImplementsPowerBuff(7)]
		public class IntensifyDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public IntensifyDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(0)]
		class MiniBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(2f);
			}
		}
		[ImplementsPowerBuff(1)]
		class FieldBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(0.5f);
			}
		}
	}
	#endregion

	//Complete
	#region WaveOfForce
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.WaveOfForce)]
	public class WizardWaveOfForce : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			UsePrimaryResource(ScriptFormula(26));
			if (Rune_A > 0) StartCooldown(5f);
			else StartCooldown(1f);     //spamming will bug the lengthy catsing animation

			yield return WaitSeconds(0.350f);    //wait for wizard to land

			int _projectilesAffected = 0;
			if (Rune_A > 0)                     //Impactful Wave
			{
				var projectiles = User.GetObjectsInRange<Projectile>(ScriptFormula(1));
				foreach (var proj in projectiles)
				{
					_projectilesAffected++;
					if (proj.Context.User == User) continue;

					if (proj.Context.User is Player)    //do not repel players projectiles
					{
						proj.Destroy();
						continue;
					}

					var newProj = new Projectile(this, proj.ActorSNO.Id, proj.Position);
					newProj.Position.Z = proj.Position.Z;
					if (proj.OnUpdate != null) newProj.OnUpdate = proj.OnUpdate;
					if (proj.OnCollision != null) newProj.OnCollision = proj.OnCollision;

					newProj.Launch(PowerMath.TranslateDirection2D(User.Position, proj.Position, User.Position, 50f), 1.5f);
					proj.Destroy();
				}
			}

			User.PlayEffectGroup(RuneSelect(19356, 82649, 215399, 215400, 215404, 215403));

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(1));
			attack.AddWeaponDamage(ScriptFormula(2), Rune_C > 0 ? DamageType.Fire : (Rune_B > 0 ? DamageType.Lightning : DamageType.Arcane));
			attack.OnHit = hitPayload =>
			{
				if (Rune_A > 0)         //Impactful Wave
				{
					if (!HasBuff<KnockbackBuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new KnockbackBuff(10f));
					if (!HasBuff<DebuffSlowed>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(18), WaitSeconds(ScriptFormula(17))));
				}
				if (Rune_B > 0)     //Static Pulse, done in HitPayload
				{
					if (!HasBuff<StaticPulseDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new StaticPulseDebuff(ScriptFormula(20), WaitSeconds(ScriptFormula(22))));
				}
				if (Rune_D > 0)     //Arcane Attunement, done in HitPayload
				{
					AddBuff(User, new AttuneBuff());
				}
				if (Rune_E > 0)         //Debilitating Force
				{
					if (!HasBuff<DebilitatingDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebilitatingDebuff(ScriptFormula(33), WaitSeconds(ScriptFormula(32))));
				}
			};
			attack.Apply();
			if (_projectilesAffected >= 8)
				(User as Player).GrantAchievement(74987243307584);
			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class DebilitatingDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public DebilitatingDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == Target)
					(payload as HitPayload).TotalDamage *= 1 - Percentage;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(0, true)]
		public class AttuneBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(2f);
				MaxStackCount = 10;         //4% per stack
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//_AddAmp();	
				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
			private void _AddAmp()
			{
			}
		}

		[ImplementsPowerBuff(2)]
		public class StaticPulseDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public StaticPulseDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region ExplosiveBlast
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.ExplosiveBlast)]
	public class ExplosiveBlast : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			Vector3D blastspot = new Vector3D(User.Position);
			Actor blast = SpawnProxy(blastspot);

			if (Rune_A > 0)         //Short Fuse
			{
				UsePrimaryResource(ScriptFormula(15));
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			}
			else
			{
				UsePrimaryResource(ScriptFormula(15));
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
				User.PlayEffectGroup(89449);
			}

			yield return WaitSeconds(ScriptFormula(5));

			if (Rune_C > 0)         //Time Bomb
			{
				SpawnEffect(61419, blastspot);
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
				attack.AddWeaponDamage(ScriptFormula(0), DamageType.Arcane);
				attack.Apply();
				yield break;
			}
			IEnumerable<TickTimer> subScript;
			if (Rune_E > 0)             //Chain Reaction
				subScript = _RuneE();
			else
				//NoRune will actually do the animation and formulas for A,B,D,and NoRune
				subScript = _NoRune();

			foreach (var timeout in subScript)
				yield return timeout;
		}
		IEnumerable<TickTimer> _NoRune()        //Short Fuse, Obliterate, Unleashed
		{
			SpawnEffect(RuneSelect(61419, 61419, 192210, -1, 192211, -1), User.Position);
			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
			attack.AddWeaponDamage(ScriptFormula(0), Rune_A > 0 ? DamageType.Fire : DamageType.Arcane);
			attack.Apply();
			yield break;
		}
		IEnumerable<TickTimer> _RuneE()     //Chain Reaction
		{
			for (int i = 0; i < ScriptFormula(8); ++i)
			{
				SpawnEffect(61419, User.Position);
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
				attack.AddWeaponDamage(ScriptFormula(0), DamageType.Fire);
				attack.Apply();
				yield return WaitSeconds(ScriptFormula(14));
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region ArcaneTorrent
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.ArcaneTorrent)]
	public class WizardArcaneTorrent : ChanneledSkill
	{
		private Actor _targetProxy = null;
		//private Actor _userProxy = null;
		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.3f;

			_targetProxy = SpawnEffect(RuneSelect(134595, 170443, 170285, 166130, 170590, 134595), TargetPosition, 0, WaitInfinite());
		}
		public override void OnChannelClose()
		{
			_targetProxy.Destroy();
		}
		public override void OnChannelUpdated()
		{

		}
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(ScriptFormula(22));

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 50f));
			if (Rune_E > 0)         //Death Blossom
				TargetPosition = RandomDirection(User.Position, 5f, 25f);

			AddBuff(User, new CastEffect());
			var userProxy = SpawnProxy(User.Position, WaitInfinite());
			userProxy.PlayEffectGroup(RuneSelect(134442, 170263, 170264, 170569, 170572, 164077), _targetProxy);

			Vector3D laggyPosition = new Vector3D(TargetPosition);

			if (Rune_C > 0)         //Arcane Mines
			{
				/*if (User.World.GetActorsBySNO(166130).Count >= 6)
				{
					User.World.GetActorsBySNO(166130).First().Destroy();
				}*/

				var mine = new EffectActor(this, 166130, TargetPosition);
				mine.Timeout = WaitSeconds(10f);
				mine.Scale = 1f;
				mine.UpdateDelay = 1.5f;
				mine.OnUpdate = () =>
				{
					if (mine.UpdateDelay == 1.5f)   //2 sec delay after spawn
					{
						mine.UpdateDelay = 2f;
						return;
					}
					if (mine.UpdateDelay == 2f) mine.UpdateDelay = 1f;

					var targets = GetEnemiesInRadius(mine.Position, 5f).FilterByType<Monster>();
					if (targets.Actors.Count > 0 && targets != null)
					{
						mine.PlayEffectGroup(171183);
						AttackPayload mine_attack = new AttackPayload(this);
						mine_attack.Targets = GetEnemiesInRadius(mine.Position, 10f);
						mine_attack.AddWeaponDamage(ScriptFormula(29) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Arcane);
						mine_attack.OnHit = hitPayload =>
						{
							if (!HasBuff<DebuffSlowed>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffSlowed(ScriptFormula(34), WaitSeconds(ScriptFormula(35))));
						};
						mine_attack.Apply();
						mine.Destroy();
					}
				};
				mine.Spawn();
				yield break;
			}

			//yield return WaitSeconds(0.8f);
			// update proxy target delayed so animation lines up with explosions a bit better
			if (IsChannelOpen)
				TranslateEffect(_targetProxy, laggyPosition, 8f);

			AttackPayload attack = new AttackPayload(this);
			attack.Targets = GetEnemiesInRadius(laggyPosition, 5f);
			attack.AddWeaponDamage((Rune_E > 0 ? ScriptFormula(17) : ScriptFormula(0)) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Arcane);
			attack.OnHit = hitPayload =>
			{
				if (Rune_A > 0)         //Disruption
				{
					if (!HasBuff<Crimson_DestablizedEffect>(hitPayload.Target))
						AddBuff(hitPayload.Target, new Crimson_DestablizedEffect());
				}
				if (Rune_B > 0 && Rand.NextDouble() < ScriptFormula(12))        //Cascade
				{
					var cascadeTargets = GetEnemiesInRadius(hitPayload.Target.Position, ScriptFormula(24)).Actors.Where(i => i != hitPayload.Target);
					if (cascadeTargets.Count() == 0) return;

					var proj = new Projectile(this, 170268, hitPayload.Target.Position);
					proj.Position.Z += 5f;
					proj.OnCollision = (hit) =>
					{
						if (hit == null || hit == hitPayload.Target) return;
						hit.PlayEffectGroup(RuneSelect(19308, 130020, 215580, -1, 216056, -1));
						WeaponDamage(GetEnemiesInRadius(hit.Position, 5f), ScriptFormula(13) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Arcane);
						proj.Destroy();
					};
					proj.Launch(cascadeTargets.FirstOrDefault().Position, ScriptFormula(23));
				}
				if (Rune_D > 0)         //Power Stone
				{
					if (Rand.NextDouble() < ScriptFormula(11))
						if (User is Player)
							hitPayload.Target.World.SpawnArcaneGlobe(hitPayload.Target, (User as Player), hitPayload.Target.Position);
				}
			};
			attack.Apply();

			userProxy.Destroy();
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class CastEffect : PowerBuff
		{
			public override void Init()
			{
				if (Rune_C > 0) Timeout = WaitSeconds(0.6f);
				else Timeout = WaitSeconds(0.3f);
			}
		}
		[ImplementsPowerBuff(1)]
		class Crimson_DestablizedEffect : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(6));
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					if ((payload as HitPayload).DominantDamageType == DamageType.Arcane)
					{
						(payload as HitPayload).TotalDamage *= (1f + ScriptFormula(5));
					}
				}
			}
		}
	}
	#endregion

	//Complete
	#region FrostNova
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.FrostNova)]
	public class WizardFrostNova : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			StartCooldown(ScriptFormula(3));

			if (Rune_C > 0)         //Frozen Mist
			{
				var frozenMist = SpawnEffect(RuneSelect(4402, 189047, 189048, 75631, 189049, 189050), User.Position, 0, WaitSeconds(ScriptFormula(9)));
				frozenMist.UpdateDelay = 1f;
				frozenMist.OnUpdate = () =>
				{
					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(frozenMist.Position, ScriptFormula(6));
					attack.AddWeaponDamage(ScriptFormula(11) / ScriptFormula(9), DamageType.Cold);
					attack.OnHit = hitPayload =>
					{
						if (!HasBuff<DebuffChilled>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffChilled(ScriptFormula(5), WaitSeconds(ScriptFormula(9))));
					};
					attack.Apply();
				};
				yield break;
			}

			SpawnEffect(RuneSelect(4402, 189047, 189048, 75631, 189049, 189050), User.Position);
			AttackPayload nova = new AttackPayload(this);
			nova.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(6));
			nova.OnHit = hitPayload =>
			{
				if (!HasBuff<DebuffFrozen>(hitPayload.Target))
					AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(1))));

				if (Rune_A > 0)         //Bone Chill
				{
					if (!HasBuff<BoneChillDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new BoneChillDebuff(ScriptFormula(16), WaitSeconds(2f)));
				}
				if (Rune_B > 0)         //Shatter
				{
					if (!HasBuff<ShatterDebuff>(hitPayload.Target))
						AddBuff(hitPayload.Target, new ShatterDebuff(User, ScriptFormula(14), WaitSeconds(2f)));
				}
				if (Rune_E > 0)         //Deep Freeze
					if (nova.Targets.Actors.Count() > ScriptFormula(13))
						if (!HasBuff<DeepFreezeChCBuff>(User))
							AddBuff(User, new DeepFreezeChCBuff(ScriptFormula(18), WaitSeconds(ScriptFormula(19))));
			};
			nova.Apply();

			yield break;
		}

		[ImplementsPowerBuff(3)]
		public class ShatterDebuff : PowerBuff
		{
			public Actor Caster = null;
			private float Chance = 0f;
			public ShatterDebuff(Actor caster, float chance, TickTimer timeout)
			{
				Chance = chance;
				Caster = caster;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is DeathPayload && payload.Target == Target)
					if (Rand.NextDouble() < Chance)
					{
						SpawnEffect(189048, payload.Target.Position);
						AttackPayload nova = new AttackPayload(this);
						nova.Targets = GetEnemiesInRadius(payload.Target.Position, 20f);
						nova.OnHit = hitPayload =>
						{
							if (!HasBuff<DebuffFrozen>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(2f)));

							if (!HasBuff<ShatterDebuff>(hitPayload.Target))
								AddBuff(hitPayload.Target, new ShatterDebuff(Caster, Chance, WaitSeconds(2f)));
							else hitPayload.Target.World.BuffManager.GetFirstBuff<ShatterDebuff>(hitPayload.Target).Extend(60);
						};
						nova.Apply();
					}
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(5)]
		public class DeepFreezeChCBuff : PowerBuff
		{
			public float Percentage = 0f;
			public DeepFreezeChCBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Weapon_Crit_Chance] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Weapon_Crit_Chance] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(4)]
		public class BoneChillDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public BoneChillDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Target == Target)
					(payload as HitPayload).TotalDamage *= 1 + Percentage;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region Blizzard
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.Blizzard)]
	public class WizardBlizzard : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			UsePrimaryResource(ScriptFormula(19));
			var blizzPoint = SpawnProxy(PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 45f)));

			var blizzard = SpawnEffect(6519, blizzPoint.Position, 0, WaitSeconds(ScriptFormula(4) + User.Attributes[GameAttribute.Power_Duration_Increase, 30680]));
			blizzard.UpdateDelay = 1f;
			blizzard.OnUpdate = () =>
			{
				foreach (var target in GetEnemiesInRadius(blizzard.Position, ScriptFormula(3)).Actors)
				{
					var hasBlizz = false;
					foreach (var buff in target.World.BuffManager.GetAllBuffs(target).Keys)
					{
						if (buff is BlizzDmgBuff)
							if ((buff as BlizzDmgBuff).Caster == User)      //Blizzards from different wizards should stack
							{
								(buff as BlizzDmgBuff).Extend(60);
								hasBlizz = true;
								break;
							}
					}

					if (!hasBlizz)
						AddBuff(target, new BlizzDmgBuff(User, ScriptFormula(0) / Math.Max(ScriptFormula(4), 1f), WaitSeconds(1f)));
				}
			};
			yield break;
		}

		[ImplementsPowerBuff(3)]
		public class BlizzDmgBuff : PowerBuff
		{
			const float DamageRate = 1f;
			TickTimer DamageTimer = null;
			private float Multiplier = 0;
			public Actor Caster = null;
			public BlizzDmgBuff(Actor caster, float multiplier, TickTimer timeout)
			{
				Caster = caster;
				Timeout = timeout;
				Multiplier = multiplier;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (DamageTimer == null || DamageTimer.TimedOut)
				{
					DamageTimer = WaitSeconds(DamageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(Multiplier, DamageType.Cold);
					attack.OnHit = hitPayload =>
					{
						if (!HasBuff<DebuffChilled>(hitPayload.Target))
						{
							if (Rune_C > 0)     //Grasping Chill
								AddBuff(hitPayload.Target, new DebuffChilled(0.8f, WaitSeconds(3f)));
							else AddBuff(hitPayload.Target, new DebuffChilled(0.5f, WaitSeconds(3f)));
						}

						if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(10))    //Frozen Solid	
							if (!HasBuff<DebuffFrozen>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(11))));
					};
					attack.Apply();
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region RayOfFrost
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Offensive.RayOfFrost)]
	public class WizardRayOfFrost : ChanneledSkill
	{
		const float MaxBeamLength = 40f;
		private Actor _beamEnd;

		private Vector3D _calcBeamEnd(float length)
		{
			return PowerMath.TranslateDirection2D(User.Position, TargetPosition,
												  new Vector3D(User.Position.X, User.Position.Y, TargetPosition.Z),
												  length);
		}

		public override void OnChannelOpen()
		{
			this.EffectsPerSecond = 0.3f;

			if (Rune_B > 0)
			{
				AddBuff(User, new IceDomeBuff());
			}
			else
			{
				_beamEnd = SpawnEffect(6535, User.Position, 0, WaitInfinite());
				User.AddComplexEffect((Rune_E > 0) ? 149879 : 19327, _beamEnd);						// Rune E uses a special beam
				User.AddComplexEffect(RuneSelect(-1, 149835, -1, 149836, 149869, -1), _beamEnd);	// Runes A, C and D add effects on top of the standard beam
			}
		}

		public override void OnChannelClose()
		{
			if (_beamEnd != null)
				_beamEnd.Destroy();
		}

		public override void OnChannelUpdated()
		{
			User.TranslateFacing(TargetPosition);

			if (Rune_B > 0)
			{
				AddBuff(User, new IceDomeBuff());
			}
		}

		public override IEnumerable<TickTimer> Main()
		{
			// Rune_D resource mod calculated in SF_19
			// SF19 is divided by 2 so multiply by 2 here
			UsePrimaryResource(ScriptFormula(19) * 2f);

			AttackPayload attack = new AttackPayload(this);
			if (Rune_B > 0)         //Sleet Storm
			{
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(7));
				attack.AddWeaponDamage(ScriptFormula(6) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Cold);
				attack.OnHit = hit =>
				{
					AddBuff(hit.Target, new DebuffChilled(ScriptFormula(14), WaitSeconds(ScriptFormula(4))));
				};
			}
			else
			{
				// Select first actor beam hits, or make max beam length
				Vector3D attackPos;
				var beamTargets = GetEnemiesInBeamDirection(User.Position, TargetPosition, MaxBeamLength, ScriptFormula(10));
				if (beamTargets.Actors.Count > 0)
				{
					Actor target = beamTargets.GetClosestTo(User.Position);
					attackPos = target.Position + new Vector3D(0, 0, 5f);  // fix height for beam end
					attack.Targets = GetEnemiesInRadius(target.Position, 5f);
				}
				else
				{
					attackPos = _calcBeamEnd(MaxBeamLength);
				}

				// update _beamEnd actor
				_beamEnd.MoveSnapped(attackPos, 0f);

				// all runes other than B seem to share the same weapon damage.
				attack.AddWeaponDamage(ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total], DamageType.Cold);

				attack.OnHit = hit =>
				{
					if (Rune_A > 0)         //Snow Blast, done in HitPayload
						if (!HasBuff<SnowBlastDebuff>(hit.Target))
							AddBuff(hit.Target, new SnowBlastDebuff(ScriptFormula(27), WaitSeconds(ScriptFormula(1))));

					if (Rune_C > 0 && Rand.NextDouble() < ScriptFormula(34))            //Numb
						if (!HasBuff<DebuffFrozen>(hit.Target))
							AddBuff(hit.Target, new DebuffFrozen(WaitSeconds(ScriptFormula(33))));

					if (!HasBuff<DebuffChilled>(hit.Target))
						AddBuff(hit.Target, new DebuffChilled(ScriptFormula(14), WaitSeconds(ScriptFormula(4))));
				};

				if (Rune_E > 0)         //Black Ice
				{
					attack.OnDeath = death =>
					{
						var icepool = SpawnEffect(148634, death.Target.Position, 0, WaitSeconds(ScriptFormula(8)));
						icepool.PlayEffectGroup(149879);
						icepool.UpdateDelay = 1f;
						icepool.OnUpdate = () =>
						{
							WeaponDamage(GetEnemiesInRadius(icepool.Position, 3f), ScriptFormula(3), DamageType.Cold);
						};
					};
				}
			}

			attack.Apply();
			yield break;
		}

		[ImplementsPowerBuff(5)]
		public class SnowBlastDebuff : PowerBuff
		{
			public float Percentage = 0f;
			public SnowBlastDebuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				return false;
			}
			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(1)]
		class IceDomeBuff : PowerBuff
		{
			//Sleet Storm
			public override void Init()
			{
				Timeout = WaitSeconds(0.3f);
			}
		}
	}
	#endregion

	//Complete
	#region Teleport
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.Teleport)]
	public class WizardTeleport : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
			{
				//Logger.Trace("Tried to Teleport to unwalkable location");
				User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));

				TeleRevertBuff buff = User.World.BuffManager.GetFirstBuff<TeleRevertBuff>(User);
				if (buff != null)
				{
					yield return WaitSeconds(0.3f);
					User.Teleport(buff.OrigSpot);
					User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));
					buff.Remove(); // Ensures that you can only revert the teleport once.
				}
				yield break;
			}

			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			if (User is Player && User.Attributes[GameAttribute.Hitpoints_Cur] < (User.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.05f))
				(User as Player).GrantAchievement(74987243307587);

			if (!(Rune_E > 0 || Rune_D > 0))
			{
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
			}

			if (Rune_D > 0)         //Reversal
			{
				TeleRevertBuff buff = User.World.BuffManager.GetFirstBuff<TeleRevertBuff>(User);
				if (buff != null)
				{
					User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));
					yield return WaitSeconds(0.3f);
					User.Teleport(buff.OrigSpot);
					User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));
					buff.Remove(); // Ensures that you can only revert the teleport once.
				}
				else
				{
					Vector3D OrigSpot;
					Actor OrigTele;
					OrigSpot = new Vector3D(User.Position.X, User.Position.Y, User.Position.Z);
					OrigTele = SpawnProxy(OrigSpot, WaitSeconds(ScriptFormula(18)));
					OrigTele.PlayEffectGroup(RuneSelect(170231, 205685, 205684, 191913, 192074, 192151));
					OrigTele.PlayEffectGroup(206679);
					AddBuff(User, new TeleRevertBuff(OrigSpot, OrigTele));
					yield return WaitSeconds(0.3f);
					User.Teleport(TargetPosition);
					User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));
				}
				yield break;
			}

			SpawnProxy(User.Position).PlayEffectGroup(RuneSelect(170231, 205685, 205684, 191913, 192074, 192151));  // alt cast efg: 170231
			yield return WaitSeconds(0.3f);
			User.Teleport(TargetPosition);
			User.PlayEffectGroup(RuneSelect(170232, 170232, 170232, 192053, 192080, 192152));

			if (Rune_A > 0)     //Calamity
			{
				User.PlayEffectGroup(170289);
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(1));
				attack.AddWeaponDamage(ScriptFormula(2), DamageType.Arcane);
				attack.OnHit = hitPayload =>
				{
					if (!HasBuff<DebuffStunned>(hitPayload.Target))
						AddBuff(hitPayload.Target, new DebuffStunned(WaitSeconds(ScriptFormula(3))));
				};
				attack.Apply();
				yield break;
			}

			if (Rune_B > 0)     //Fracture
			{
				int maxImages = (int)ScriptFormula(8);
				List<Actor> Images = new List<Actor>();
				for (int i = 0; i < maxImages; i++)
				{
					var Image = new MirrorImageMinion(this.World, this, i, ScriptFormula(9));
					Image.Brain.DeActivate();
					Image.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
					Image.Attributes[GameAttribute.Untargetable] = true;
					Image.EnterWorld(Image.Position);
					Images.Add(Image);
					yield return WaitSeconds(0.2f);
				}
				foreach (Actor Image in Images)
				{
					(Image as Minion).Brain.Activate();
					Image.Attributes[GameAttribute.Untargetable] = false;
					Image.Attributes.BroadcastChangedIfRevealed();
				}
				yield break;
			}

			if (Rune_C > 0 && !HasBuff<TeleDmgReductionBuff>(User))     //Safe Passage
				AddBuff(User, new TeleDmgReductionBuff());

			if (Rune_E > 0 && !HasBuff<TeleCoolDownBuff>(User))         //Wormhole
				AddBuff(User, new TeleCoolDownBuff());
		}
		[ImplementsPowerBuff(1)]
		class TeleDmgReductionBuff : PowerBuff
		{
			public override void Init() { Timeout = WaitSeconds(ScriptFormula(15)); }

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//gameattribute damage reduction, Absorb should do the same thing?
				Target.Attributes[GameAttribute.Damage_Absorb_Percent] += ScriptFormula(14);
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Damage_Absorb_Percent] -= ScriptFormula(14);
				Target.Attributes.BroadcastChangedIfRevealed();

			}
		}
		[ImplementsPowerBuff(5)]
		class TeleRevertBuff : PowerBuff
		{
			public Vector3D OrigSpot;
			public Actor OrigTele;

			public TeleRevertBuff(Vector3D OrigSpot, Actor OrigTele)
			{
				this.OrigSpot = OrigSpot;
				this.OrigTele = OrigTele;
			}

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override void Remove()
			{
				Timeout.Stop();
				//OrigTele.Destroy();  --   Removes the voidzone effect as though, but also throws an exception. 
				//						  Perhaps one cannot Destroy() a proxy actor, 
				//						  but is there any other way to quit the effectgroup early?
				base.Remove();
				StartCooldown(ScriptFormula(20));
			}
		}
		[ImplementsPowerBuff(5)]
		class TeleCoolDownBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(16));
			}

			public override void Remove()
			{
				base.Remove();
				StartCooldown(ScriptFormula(20));
			}
		}
	}
	#endregion

	//Complete
	#region Icearmor
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.IceArmor)]
	public class IceArmor : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			AddBuff(User, new IceArmorBuff());
			RemoveBuffs(User, 74499); //StormArmor
			RemoveBuffs(User, 86991); //EnergyArmor
			if (Rune_C > 0)     //Frozen Storm
			{
				yield return WaitSeconds(1f);
				AddBuff(User, new FrozenRingBuff());
			}

			yield break;
		}

		//0 = IceArmor, 1 = Rune_C, 2 = buff_switch
		[ImplementsPowerBuff(0)]
		public class IceArmorBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(3));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] += ScriptFormula(10);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					if (PowerMath.Distance2D(payload.Target.Position, payload.Context.User.Position) > 12f) return;

					if (!payload.Context.User.Attributes[GameAttribute.Freeze_Immune])
					{
						if (!HasBuff<DebuffFrozen>(payload.Context.User))
							AddBuff(payload.Context.User, new DebuffFrozen(WaitSeconds(ScriptFormula(4))));
					}
					else if (!HasBuff<DebuffChilled>(payload.Context.User))
						AddBuff(payload.Context.User, new DebuffChilled(0.6f, WaitSeconds(ScriptFormula(4))));

					if (Rune_A > 0)     //Jagged Ice
						WeaponDamage(payload.Context.User, ScriptFormula(0), DamageType.Cold);

					if (Rune_D > 0)         //Crystallize
						AddBuff(User, new BonusStackEffect());

					if (Rune_E > 0 && Rand.NextDouble() < ScriptFormula(12))        //Ice Reflect
					{
						payload.Context.User.PlayEffectGroup(19321);
						AttackPayload frostNova = new AttackPayload(this);
						frostNova.Targets = GetEnemiesInRadius(payload.Context.User.Position, ScriptFormula(6));
						frostNova.AddWeaponDamage(ScriptFormula(13), DamageType.Cold);
						frostNova.OnHit = hitPayload =>
						{
							if (!HasBuff<DebuffFrozen>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffFrozen(WaitSeconds(3f)));
						};
						frostNova.Apply();
					}
				}
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					if (Rune_B > 0)     //Chilling Aura
					{
						AttackPayload chillingAura = new AttackPayload(this);
						chillingAura.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(7));
						chillingAura.OnHit = (hitPayload) =>
						{
							if (HasBuff<DebuffFrozen>(hitPayload.Target)) return;

							if (!HasBuff<DebuffChilled>(hitPayload.Target))
								AddBuff(hitPayload.Target, new DebuffChilled(0.8f, WaitSeconds(1f)));
							else
								hitPayload.Target.World.BuffManager.GetFirstBuff<DebuffChilled>(hitPayload.Target).Extend(60);
						};
						chillingAura.Apply();
					}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Damage_Percent_Reduction_From_Melee] += ScriptFormula(10);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}

		//Rune_D Crystallize
		[ImplementsPowerBuff(2, true)]
		class BonusStackEffect : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(27));
				MaxStackCount = (int)ScriptFormula(11);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(26);
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;
				base.Stack(buff);

				if (!stacked) return true;

				User.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(26);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Armor_Bonus_Percent] -= StackCount * ScriptFormula(26);
				User.Attributes.BroadcastChangedIfRevealed();
			}
		}
		//Rune_C Frozen Storm
		[ImplementsPowerBuff(1)]
		class FrozenRingBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(3));
			}
			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload chillingAura = new AttackPayload(this);
					chillingAura.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(7));
					chillingAura.AddWeaponDamage(ScriptFormula(9), DamageType.Cold);
					chillingAura.OnHit = hitPayload =>
					{
						if (HasBuff<DebuffFrozen>(hitPayload.Target)) return;

						if (!HasBuff<DebuffChilled>(hitPayload.Target))
							AddBuff(hitPayload.Target, new DebuffChilled(0.6f, WaitSeconds(1f)));
						else
							hitPayload.Target.World.BuffManager.GetFirstBuff<DebuffChilled>(hitPayload.Target).Extend(60);
					};
					chillingAura.Apply();
				}
				return false;
			}
		}
	}
	#endregion

	//Complete
	#region ShockPulse
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Signature.ShockPulse)]
	public class WizardShockPulse : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			if ((User as Player).SkillSet.HasPassive(208493)) //Prodigy (wizard)
				GeneratePrimaryResource(5f);

			if (HasBuff<EnergyTwister.TwisterBuff>(User))   //EnergyTwister -> Storm Chaser
			{
				var mult = User.World.BuffManager.GetFirstBuff<EnergyTwister.TwisterBuff>(User).StackCount;
				var proj = new Projectile(this, 210896, User.Position);
				proj.Position.Z += 5f;
				proj.RadiusMod = 1.5f;
				proj.OnCollision = hit =>
				{
					if (hit == null) return;
					WeaponDamage(hit, 1.96f * mult, DamageType.Lightning);
				};
				proj.Launch(TargetPosition, 0.8f);
				RemoveBuffs(User, SkillsSystem.Skills.Wizard.Offensive.EnergyTwister);
			}

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 35f));

			if ((User as Player).SkillSet.HasPassive(208823) && Rand.NextDouble() < GetProcCoefficient()) //ArcaneDynamo (wizard)
				AddBuff(User, new DynamoBuff());

			User.PlayEffectGroup(67099); // cast effect
			for (int n = 0; n < ((Rune_B > 0 || Rune_C > 0) ? 1 : 3); ++n)
			{
				var proj = new Projectile(this, RuneSelect(176247, 176287, 176653, 201526, 176248, 176356), User.Position);
				proj.OnCollision = (hit) =>
				{
					if (Rune_B <= 0 && Rune_C <= 0)
						SpawnEffect(RuneSelect(176247, 176287, 176653, 201526, 176248, 176356), new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f)); // impact effect (fix height)

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = new TargetList();
					if (Rune_B > 0)     //Living Lightning
						attack.Targets = GetEnemiesInRadius(hit.Position, ScriptFormula(21));
					else
						attack.Targets.Actors.Add(hit);

					attack.AddWeaponDamage(ScriptFormula(4), Rune_E > 0 ? DamageType.Cold : (Rune_D > 0 ? DamageType.Arcane : (Rune_A > 0 ? DamageType.Fire : DamageType.Lightning)));
					attack.OnHit = hitPayload =>
					{
						if (Rune_D > 0) GeneratePrimaryResource(ScriptFormula(15));     //Power Affinity
					};
					if (Rune_E > 0)     //Explosive Bolts
					{
						attack.OnDeath = DeathPayload =>
						{
							SpawnProxy(Target.Position).PlayEffectGroup(18991);
							WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(8)), ScriptFormula(9), DamageType.Cold);
						};
					}
					attack.Apply();
					if (!(Rune_B > 0 || Rune_C > 0))
						proj.Destroy();
				};
				var destination = ((Rune_B > 0 || Rune_C > 0) ? TargetPosition : PowerMath.GenerateSpreadPositions(User.Position, TargetPosition, 5f, 9)[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, 9)]);
				proj.Launch(destination, ScriptFormula(6));
				//WaitSeconds(ScriptFormula(25));
			}
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class DeathTriggerBuff : PowerBuff
		{

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(9));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}
			public override void Remove() { base.Remove(); }
		}
	}
	#endregion

	//Complete
	#region StormArmor
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.StormArmor)]
	public class StormArmor : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new StormArmorBuff());
			RemoveBuffs(User, 73223); //IceArmor
			RemoveBuffs(User, 86991); //EnergyArmor
			if (Rune_D > 0)     //Power of the Storm
			{
				AddBuff(User, new GoldenBuff());
			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class StormArmorBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				//User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] += ScriptFormula(22);
				return true;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.Targets = GetEnemiesInRadius(User.Position, 20f, 8);
					foreach (var target in attack.Targets.Actors)
					{
						target.PlayEffectGroup(312568);
					}

					attack.AddWeaponDamage(ScriptFormula(1), DamageType.Lightning); //Rune A Thunderstorm included here
					attack.AutomaticHitEffects = false;     //no procs from magic weapon here
					attack.Apply();
				}
				return false;
			}
			public override void OnPayload(Payload payload)
			{
				//Shocking Aspect
				if (Rune_E > 0 && payload.Context.User == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage)
					if (Rand.NextDouble() < 0.5f * payload.Context.GetProcCoefficient())
						if ((payload as HitPayload).IsCriticalHit && (payload as HitPayload).AutomaticHitEffects)
						{
							AttackPayload attack = new AttackPayload(this);
							attack.SetSingleTarget(GetEnemiesInRadius(Target.Position, 15f).GetClosestTo(Target.Position));
							attack.AddWeaponDamage(ScriptFormula(15), DamageType.Lightning);
							attack.AutomaticHitEffects = false;
							attack.Apply();
							return;
						}

				//Scramble
				if (Rune_B > 0 && payload.Target == Target && payload is HitPayload && (payload as HitPayload).AutomaticHitEffects)
				{
					AddBuff(User, new IndigoBuff());
					//AddBuff(User, new MovementBuff(ScriptFormula(14), WaitSeconds(ScriptFormula(20))));
				}

				//Reactive Armor
				if (Rune_C > 0 && payload.Target == Target && payload is HitPayload && (payload as HitPayload).AutomaticHitEffects)
					if (PowerMath.Distance2D(payload.Target.Position, payload.Context.User.Position) < 25)
					{
						//projectile? ScriptFormula(3) is speed.
						User.AddRopeEffect(186883, payload.Context.User);
						AttackPayload attack = new AttackPayload(this);
						attack.SetSingleTarget(payload.Context.User);
						attack.AddWeaponDamage(ScriptFormula(9), DamageType.Lightning);
						attack.AutomaticHitEffects = false;
						attack.Apply();
					}
			}

			public override void Remove()
			{
				base.Remove();
				//User.Attributes[GameAttribute.Hitpoints_Regen_Per_Second] -= ScriptFormula(22);
			}
		}
		[ImplementsPowerBuff(1)]
		class TeslaBuff : PowerBuff
		{

			public override void Init()
			{
				Timeout = WaitSeconds(1f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		class IndigoBuff : PowerBuff
		{
			public float Percentage = 0f;
			public override void Init()
			{
				Percentage = ScriptFormula(14);
				Timeout = WaitSeconds(ScriptFormula(23));
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= Percentage;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
		[ImplementsPowerBuff(3)]
		class GoldenBuff : PowerBuff        //Power of the Storm
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				User.Attributes[GameAttribute.Resource_Cost_Reduction_Amount] += (int)ScriptFormula(7);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				User.Attributes[GameAttribute.Resource_Cost_Reduction_Amount] -= (int)ScriptFormula(7);
				User.Attributes.BroadcastChangedIfRevealed();

				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region DiamondSkin
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.DiamondSkin)]
	public class DiamondSkin : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			//No Resource Cost
			AddBuff(User, new DiamondSkinBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class DiamondSkinBuff : PowerBuff
		{
			float HPTreshold = 0f;
			bool DidBlast = false;

			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));    //Rune B Enduring Skin included here
				HPTreshold = User.Attributes[GameAttribute.Hitpoints_Max_Total] * (Rune_C > 0 ? 0.6f : 0.3f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0)     //Sleek Shell
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] += ScriptFormula(1);
				if (Rune_D > 0)     //Prism
					User.Attributes[GameAttribute.Resource_Cost_Reduction_Amount] += (int)ScriptFormula(4);

				User.Attributes[GameAttribute.Has_Look_Override, 0x061F7489] = true;//0x061F7489;
				User.Attributes.BroadcastChangedIfRevealed();
				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload && HPTreshold > 0)
				{
					float dmg = (payload as HitPayload).TotalDamage;
					(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
					HPTreshold -= dmg;
					if (HPTreshold <= 0)
						User.World.BuffManager.RemoveBuff(User, this);
				}
			}

			public override void Remove()
			{
				if (Rune_E > 0 && !DidBlast)        //Diamond Shards
				{
					DidBlast = true;
					User.PlayEffectGroup(92957);
					WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(6)), ScriptFormula(2), DamageType.Arcane);
				}
				if (Rune_A > 0)
					User.Attributes[GameAttribute.Movement_Scalar_Uncapped_Bonus] -= ScriptFormula(1);
				if (Rune_D > 0)
					User.Attributes[GameAttribute.Resource_Cost_Reduction_Amount] -= (int)ScriptFormula(4);

				User.Attributes[GameAttribute.Has_Look_Override, 0x061F7489] = false;
				User.PlayEffectGroup(RuneSelect(93077, 187716, 187805, 187822, 187831, 187851));
				User.Attributes.BroadcastChangedIfRevealed();
				base.Remove();
			}
		}
		[ImplementsPowerBuff(1)]
		class StoneSkinBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
			}
		}
		[ImplementsPowerBuff(2)]
		class StoneArmorBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(5));
			}
		}
	}
	#endregion

	//Complete
	#region SlowTime
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.SlowTime)]
	public class SlowTime : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			//No Resouce Cost
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 50f));
			var TimeWarp = new EffectActor(this, RuneSelect(6553, 112585, 112808, 112560, 112572, 112697), Rune_D > 0 ? TargetPosition : User.Position);
			TimeWarp.Timeout = WaitSeconds(ScriptFormula(0));
			TimeWarp.Scale = 1f;
			TimeWarp.UpdateDelay = 0.2f;
			TimeWarp.OnUpdate = () =>
			{
				var targets = GetEnemiesInRadius(TimeWarp.Position, 20f);
				if (targets.Actors.Count > 0)
					foreach (Actor actor in targets.Actors)
					{
						if (!HasBuff<SlowTimeDebuff>(actor))
							AddBuff(actor, new SlowTimeDebuff(ScriptFormula(3), WaitSeconds(0.2f + ScriptFormula(12))));

						if (Rune_A > 0 && !HasBuff<AttackDamageBuff>(actor))        //Time Warp
							AddBuff(actor, new AttackDamageBuff());

						if (Rune_B > 0)     //Event Horizon	
							if (PowerMath.Distance2D(TimeWarp.Position, actor.Position) > 18f)
								if (!HasBuff<EventHorizonBuff>(actor) && !HasBuff<DebuffStunned>(actor))
								{
									AddBuff(actor, new EventHorizonBuff(WaitSeconds(10f)));     //A lot of bubbles can be spawned with Illusionist, so a cooldown buff here
									AddBuff(actor, new DebuffStunned(WaitSeconds(3f)));
								}
					}

				var projectiles_around = TimeWarp.GetObjectsInRange<Projectile>(40f).Where(p => PowerMath.Distance2D(p.Position, TimeWarp.Position) > 20f);
				if (projectiles_around.Count() > 0)
					foreach (Projectile actor in projectiles_around)
						if (actor.Slowed)
						{
							actor.Slowed = false;
							actor.Attributes[GameAttribute.Projectile_Speed] *= 10f;
						}

				var projectiles = TimeWarp.GetObjectsInRange<Projectile>(20f);
				if (projectiles.Count > 0)
					foreach (Projectile actor in projectiles)
						if (!actor.Slowed)
						{
							TimeWarp.UtilityValue++;
							actor.Slowed = true;
							actor.Attributes[GameAttribute.Projectile_Speed] /= 10f;
						}

				if (Rune_E > 0)     //Stretch Time
				{
					var friendlytargets = GetAlliesInRadius(TimeWarp.Position, 20f);
					friendlytargets.Actors.Add(User);
					if (friendlytargets.Actors.Count <= 0) return;

					foreach (Actor actor in friendlytargets.Actors)
					{
						if (!HasBuff<SpeedBuff>(actor))
							AddBuff(actor, new SpeedBuff(ScriptFormula(16), WaitSeconds(0.2f)));
						else actor.World.BuffManager.GetFirstBuff<SpeedBuff>(actor).Extend(12);
					}

				}
				if (TimeWarp.UtilityValue >= 20)
					(User as Player).GrantAchievement(74987243307583);
			};
			TimeWarp.Spawn();
			yield break;
		}

		[ImplementsPowerBuff(1)]
		class AttackDamageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(0.2f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(8);
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(8);
			}
		}

		[ImplementsPowerBuff(4)]
		public class EventHorizonBuff : PowerBuff
		{
			public EventHorizonBuff(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
	}
	#endregion

	//Complete
	#region EnergyArmor
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.EnergyArmor)]
	public class EnergyArmor : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			AddBuff(User, new EnergyArmorBuff());
			RemoveBuffs(User, 73223); //IceArmor
			RemoveBuffs(User, 74499); //StormArmor
			yield break;
		}

		[ImplementsPowerBuff(0)]
		public class EnergyArmorBuff : PowerBuff
		{
			bool AbsorbReady = false;
			TickTimer AbsorbCDTimer = null;     //Absorbtion effect's internal cooldown
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(0));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0)     //Prismatic Armor
					Target.Attributes[GameAttribute.Resistance_Percent_All] += ScriptFormula(4);

				if (Rune_B > 0)     //Energy Tap
					Target.Attributes[GameAttribute.Resource_Max_Bonus, 1] += ScriptFormula(6);
				else
					Target.Attributes[GameAttribute.Resource_Max_Bonus, 1] -= ScriptFormula(2);

				if (Rune_E > 0)     //Pinpoint Barrier
					Target.Attributes[GameAttribute.Weapon_Crit_Chance] += ScriptFormula(13);

				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();


				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target == Target && payload is HitPayload)
				{
					if (Rune_C > 0)     //Force Armor
						(payload as HitPayload).TotalDamage = Math.Min((payload as HitPayload).TotalDamage, Target.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(8));

					if (Rune_D > 0 && AbsorbReady)      //Absorbtion
					{
						GeneratePrimaryResource(ScriptFormula(11));
						AbsorbReady = false;
					}
				}
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (AbsorbCDTimer == null || AbsorbCDTimer.TimedOut)
				{
					AbsorbCDTimer = WaitSeconds(0.5f);
					if (!AbsorbReady) AbsorbReady = true;
				}
				return false;
			}
			public override void Remove()
			{
				base.Remove();
				if (Rune_A > 0)
					Target.Attributes[GameAttribute.Resistance_Percent_All] -= ScriptFormula(4);

				if (Rune_B > 0)
					Target.Attributes[GameAttribute.Resource_Max_Bonus, 1] -= ScriptFormula(6);
				else
					Target.Attributes[GameAttribute.Resource_Max_Bonus, 1] += ScriptFormula(2);

				if (Rune_E > 0)
					Target.Attributes[GameAttribute.Weapon_Crit_Chance] -= ScriptFormula(13);

				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(1);
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete
	#region MagicWeapon
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.MagicWeapon)]
	public class MagicWeapon : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			AddBuff(User, new MagicWeaponBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class MagicWeaponBuff : PowerBuff
		{
			bool ElectrifyReady = false;
			int ElectrifiesDone = 0;
			const int ElectrifiesCap = 3;

			bool IgniteReady = false;
			int IgnitesDone = 0;
			const int IgnitesCap = 8;
			const float Cooldown = 1f;
			TickTimer CooldownTimer = null;
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(2));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				User.PlayEffectGroup(RuneSelect(218923, 219289, 219306, 219390, 219396, 219338));
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] += ScriptFormula(14);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(14);
				User.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void Remove()
			{
				base.Remove();
				User.Attributes[GameAttribute.Damage_Weapon_Percent_Bonus] -= ScriptFormula(14);
				User.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(14);
				User.Attributes.BroadcastChangedIfRevealed();
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (CooldownTimer == null || CooldownTimer.TimedOut)
				{
					CooldownTimer = WaitSeconds(Cooldown);

					ElectrifiesDone = 0;
					ElectrifyReady = true;

					IgnitesDone = 0;
					IgniteReady = true;
				}
				return false;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Target != null && payload.Target != User && payload.Context.User == User &&
					payload is HitPayload && (payload as HitPayload).AutomaticHitEffects &&
					(payload as HitPayload).IsWeaponDamage)
				{
					HitPayload lastAttack = (HitPayload)payload;

					if (Rune_A > 0 && IgniteReady && Rand.NextDouble() < lastAttack.Context.GetProcCoefficient())       //Ignite
					{
						if (lastAttack.Target == null || lastAttack.Target.World == null) return;
						IgnitesDone++;
						if (IgnitesDone > IgnitesCap) IgniteReady = false;

						AddBuff(lastAttack.Target, new BurnTarget());
						lastAttack.Target.PlayEffectGroup(154844);
						return;
					}

					if (Rune_B > 0 && ElectrifyReady && Rand.NextDouble() < lastAttack.Context.GetProcCoefficient())        //Electrify
					{
						if (lastAttack.Target == null || lastAttack.Target.World == null) return;
						ElectrifiesDone++;
						if (ElectrifiesDone > ElectrifiesCap) ElectrifyReady = false;

						var damageMult = ScriptFormula(7);
						Actor curTarget = lastAttack.Target;

						AttackPayload arc = new AttackPayload(this);
						arc.AutomaticHitEffects = false;        //no procs and self-procs from this
						arc.SetSingleTarget(curTarget);
						arc.AddWeaponDamage(damageMult, DamageType.Lightning);
						arc.Apply();

						IList<Actor> targets = new List<Actor>() { curTarget };
						Actor nextTarget = null;
						var c = 0;
						while (targets.Count() < 3)
						{
							nextTarget = GetEnemiesInRadius(curTarget.Position, 6f).Actors.FirstOrDefault(a => !targets.Contains(a));
							if (nextTarget == null) break;

							curTarget.AddRopeEffect(186883, nextTarget);

							AttackPayload chainArc = new AttackPayload(this);
							chainArc.AutomaticHitEffects = false;       //no procs and self-procs from this
							chainArc.SetSingleTarget(curTarget);
							chainArc.AddWeaponDamage(damageMult, DamageType.Lightning);
							chainArc.Apply();

							curTarget = nextTarget;
							targets.Add(curTarget);

							c++;
							if (c > 6) break;
						}
						return;
					}

					if (Rune_C > 0 && Rand.NextDouble() < 0.35f * lastAttack.Context.GetProcCoefficient())      //Force Weapon
						if (!HasBuff<KnockbackBuff>(lastAttack.Target))
							AddBuff(lastAttack.Target, new KnockbackBuff(ScriptFormula(10)));

					if (Rune_D > 0 && Rand.NextDouble() < lastAttack.Context.GetProcCoefficient())      //Conduit
						GeneratePrimaryResource(ScriptFormula(11));

					if (Rune_E > 0 && !HasBuff<DeflectionBuff>(User))       //Deflection
						if (Rand.NextDouble() < lastAttack.Context.GetProcCoefficient())
							AddBuff(User, new DeflectionBuff(User.Attributes[GameAttribute.Hitpoints_Max_Total] * 0.04f, WaitSeconds(ScriptFormula(13))));
				}
			}
		}
		[ImplementsPowerBuff(1, true)]
		class BurnTarget : PowerBuff
		{
			float Dps = 1f;
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;


			public override void Init()
			{
				Dps = ScriptFormula(4);
				Timeout = WaitSeconds(3f);
				MaxStackCount = 3;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null)
					_damageTimer = WaitSeconds(_damageRate);

				if (_damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					AttackPayload attack = new AttackPayload(this);
					attack.AutomaticHitEffects = false;     //no procs and self-procs from this
					attack.SetSingleTarget(Target);
					attack.AddWeaponDamage(Dps * StackCount, DamageType.Fire);
					attack.Apply();
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		public class DeflectionBuff : PowerBuff
		{
			public float HPTreshold = 0f;
			public DeflectionBuff(float hpTreshold, TickTimer timeout)
			{
				HPTreshold = hpTreshold;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				//Target.Attributes.BroadcastChangedIfRevealed();
				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (HPTreshold > 0)
					if (payload is HitPayload && payload.Target == User && (payload as HitPayload).IsWeaponDamage)
					{
						float dmg = (payload as HitPayload).TotalDamage;
						(payload as HitPayload).TotalDamage -= Math.Min((payload as HitPayload).TotalDamage, HPTreshold);
						HPTreshold -= dmg;
						if (HPTreshold < 0f)
							User.World.BuffManager.RemoveBuff(User, this);
					}
			}
			public override void Remove()
			{
				base.Remove();
				//Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}
	#endregion

	//Complete.
	#region Archon
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.Archon)]
	public class Archon : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));
			yield return WaitSeconds(0.3f);

			AddBuff(User, new ArchonBuff());
			if (Rune_E > 0)     //Arcane Destruction
				WeaponDamage(GetEnemiesInRadius(User.Position, ScriptFormula(7)), ScriptFormula(5), DamageType.Arcane);
			yield break;
		}

		[ImplementsPowerBuff(2)]
		public class ArchonBuff : PowerBuff
		{
			float KillStreakBonus = 0f;
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(2));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				Target.Attributes[GameAttribute.Armor_Bonus_Percent] += ScriptFormula(3);
				Target.Attributes[GameAttribute.Resistance_Percent_All] += ScriptFormula(4);
				Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(9);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(9);

				Target.Attributes[GameAttribute.Skill_Override, 0] = 135166;
				Target.Attributes[GameAttribute.Skill_Override, 1] = 135238;
				Target.Attributes[GameAttribute.Skill_Override, 2] = 167355;
				Target.Attributes[GameAttribute.Skill_Override, 3] = Rune_B > 0 ? 135663 : -1;
				Target.Attributes[GameAttribute.Skill_Override, 4] = Rune_C > 0 ? 167648 : -1;
				Target.Attributes[GameAttribute.Skill_Override, 5] = 166616;
				Target.Attributes[GameAttribute.Skill_Override_Active] = true;
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == Target && payload is DeathPayload)
				{
					KillStreakBonus += 0.06f;
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] += 0.06f;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += 0.06f;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}

			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Armor_Bonus_Percent] -= ScriptFormula(3);
				Target.Attributes[GameAttribute.Resistance_Percent_All] -= ScriptFormula(4);
				Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(9);
				Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(9);
				if (KillStreakBonus > 0f)
				{
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= KillStreakBonus;
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= KillStreakBonus;
					KillStreakBonus = 0f;
				}
				Target.Attributes[GameAttribute.Skill_Override_Active] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(166616)]
	public class ArchonCancel : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			RemoveBuffs(User, SkillsSystem.Skills.Wizard.Utility.Archon);
			RemoveBuffs(User, 135663);
			yield break;
		}
	}

	[ImplementsPowerSNO(135238)]
	public class ArchonDisintegrationWave : ChanneledSkill
	{
		const float BeamLength = 40f;
		float DamageMult = 1f;
		private Actor _target = null;

		private void _calcTargetPosition()
		{
			// project beam end to always be a certain length
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition,
															 new Vector3D(User.Position.X, User.Position.Y, TargetPosition.Z),
															 BeamLength);
		}

		public override void OnChannelOpen()
		{
			EffectsPerSecond = 0.3f;
			DamageMult = ScriptFormula(0) * EffectsPerSecond * User.Attributes[GameAttribute.Attacks_Per_Second_Total];
			_calcTargetPosition();

			//"Disc" visuals
			if (Rune_A > 0) AddBuff(User, new WaveBuffDmg(WaitSeconds(10f)));
			else if (Rune_B > 0) AddBuff(User, new WaveBuffSlow(WaitSeconds(10f)));
			else if (Rune_C > 0) AddBuff(User, new WaveBuffTeleport(WaitSeconds(10f)));
			else if (Rune_D > 0) AddBuff(User, new WaveBuffErupt(WaitSeconds(10f)));
			else if (Rune_E > 0) AddBuff(User, new WaveBuffExplode(WaitSeconds(10f)));
			else AddBuff(User, new WaveBuffExplode(WaitSeconds(10f)));

			_target = SpawnEffect(161695, TargetPosition, 0, WaitInfinite());
			User.AddComplexEffect(RuneSelect(161575, 216981, 217257, 217128, 216983, 216963), _target);
		}

		public override void OnChannelClose()
		{
			RemoveBuffs(User, 135238);
			if (_target != null)
				_target.Destroy();
		}

		public override void OnChannelUpdated()
		{
			_calcTargetPosition();
			User.TranslateFacing(TargetPosition);
			// client updates target actor position
		}

		public override IEnumerable<TickTimer> Main()
		{
			foreach (Actor actor in GetEnemiesInRadius(User.Position, BeamLength + 10f).Actors)
			{
				if (PowerMath.PointInBeam(actor.Position, User.Position, TargetPosition, 3f))
					WeaponDamage(actor, DamageMult, DamageType.Arcane);
			}
			yield break;
		}

		[ImplementsPowerBuff(1)]
		public class WaveBuffExplode : PowerBuff        //Arcane Destruction
		{
			public WaveBuffExplode(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(2)]
		public class WaveBuffSlow : PowerBuff       //Slow Time
		{
			public WaveBuffSlow(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(3)]
		public class WaveBuffTeleport : PowerBuff       //Teleport
		{
			public WaveBuffTeleport(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(4)]
		public class WaveBuffDmg : PowerBuff        //Improved Archon
		{
			public WaveBuffDmg(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(5)]
		public class WaveBuffErupt : PowerBuff      //Pure Power
		{
			public WaveBuffErupt(TickTimer timeout)
			{
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}
	}

	[ImplementsPowerSNO(135166)]
	public class ArchonArcaneStrike : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			// calculate hit area of effect, just in front of the user
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, ScriptFormula(1));

			SpawnProxy(TargetPosition).PlayEffectGroup(164678);

			AttackPayload strike = new AttackPayload(this);
			strike.Targets = GetEnemiesInRadius(TargetPosition, ScriptFormula(1));
			strike.AddWeaponDamage(ScriptFormula(0), DamageType.Arcane);
			strike.Apply();

			yield break;
		}
	}

	[ImplementsPowerSNO(167355)]
	public class ArchonArcaneBlast : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			StartCooldown(EvalTag(PowerKeys.CooldownTime));

			User.PlayEffectGroup(166085);

			AttackPayload strike = new AttackPayload(this);
			strike.Targets = GetEnemiesInRadius(User.Position, ScriptFormula(0));
			strike.AddWeaponDamage(ScriptFormula(1), DamageType.Arcane);
			strike.Apply();

			yield break;
		}
	}

	[ImplementsPowerSNO(167648)]
	public class ArchonTeleport : Skill     //Rune C
	{
		public override IEnumerable<TickTimer> Main()
		{
			// calculate hit area of effect, just in front of the user
			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), EvalTag(PowerKeys.AttackRadius)));

			if (User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
			{
				StartCooldown(EvalTag(PowerKeys.CooldownTime));
				SpawnProxy(User.Position).PlayEffectGroup(170231);
				yield return WaitSeconds(0.3f);
				User.Teleport(TargetPosition);
				User.PlayEffectGroup(170232);
			}

			yield break;
		}
	}

	[ImplementsPowerSNO(135663)]
	public class ArchonSlowTime : Skill     //Rune B
	{
		public override IEnumerable<TickTimer> Main()
		{
			//StartCooldown(EvalTag(PowerKeys.CooldownTime));
			if (!HasBuff<SlowTimeBuff>(User)) AddBuff(User, new SlowTimeBuff());
			else RemoveBuffs(User, 135663);

			yield break;
		}

		[ImplementsPowerBuff(0)]
		class SlowTimeBuff : PowerBuff
		{
			const float _damageRate = 0.2f;
			TickTimer _damageTimer = null;
			public override void Init()
			{
				Timeout = User.World.BuffManager.GetFirstBuff<Archon.ArchonBuff>(User).Timeout;
			}

			public override bool Update()
			{
				if (base.Update())
					return true;

				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					var targets = GetEnemiesInRadius(User.Position, ScriptFormula(2));
					if (targets.Actors.Count == 0) return false;

					foreach (Actor actor in targets.Actors)
					{
						if (!HasBuff<SlowTimeDebuff>(actor))
							AddBuff(actor, new SlowTimeDebuff(ScriptFormula(3), WaitSeconds(0.2f + ScriptFormula(1))));
					}

					var projectiles_around = User.GetObjectsInRange<Projectile>(40f).Where(p => PowerMath.Distance2D(p.Position, User.Position) > 20f);
					if (projectiles_around.Count() > 0)
						foreach (Projectile actor in projectiles_around)
							if (actor.Slowed)
							{
								actor.Slowed = false;
								actor.Attributes[GameAttribute.Projectile_Speed] *= 10f;
							}

					var projectiles = User.GetObjectsInRange<Projectile>(20f);
					if (projectiles.Count() > 0)
						foreach (Projectile actor in projectiles)
							if (!actor.Slowed)
							{
								actor.Slowed = true;
								actor.Attributes[GameAttribute.Projectile_Speed] /= 10f;
							}

				}
				return false;
			}
		}
	}
	#endregion

	//Lots of Work.
	#region MirrorImage
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.MirrorImage)]
	public class MirrorImage : Skill
	{
		//Once cast, you slide randomly into one of the ___ number of spots that other images fill around a circle.
		//from there, all AI mirror images run randomly, targeting and casting spells..
		//there is no following for these I believe.
		public override IEnumerable<TickTimer> Main()
		{
			//StartCooldown(EvalTag(PowerKeys.CooldownTime));
			//UsePrimaryResource(ScriptFormula(12));
			int maxImages = (int)ScriptFormula(1);
			List<Actor> Images = new List<Actor>();
			for (int i = 0; i < maxImages; i++)
			{
				var Image = new MirrorImageMinion(this.World, this, i, ScriptFormula(2));
				Image.Brain.DeActivate();
				Image.Position = RandomDirection(User.Position, 3f, 8f); //Kind of hacky until we get proper collisiondetection
				Image.Attributes[GameAttribute.Untargetable] = true;
				Image.EnterWorld(Image.Position);
				Images.Add(Image);
				yield return WaitSeconds(0.2f);
			}
			yield return WaitSeconds(0.8f);
			foreach (Actor Image in Images)
			{
				(Image as Minion).Brain.Activate();
				Image.Attributes[GameAttribute.Untargetable] = false;
				Image.Attributes.BroadcastChangedIfRevealed();
			}
			yield break;
		}
	}
	#endregion

	//Complete
	#region Familiar
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.Familiar)]
	public class Familiar : Skill
	{
		public override IEnumerable<TickTimer> Main()
		{
			//User.PlayEffectGroup(167334);
			AddBuff(User, new FamiliarBuff());
			//AddBuff(User, new IconBuff());
			yield break;
		}


		[ImplementsPowerBuff(2)]
		class IconBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}
			public override void Remove()
			{
				base.Remove();
				RemoveBuffs(User, 99120);
			}
		}

		[ImplementsPowerBuff(0)]
		class FamiliarBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(1));
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Rune_A > 0)     //Sparkflint
				{
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] += ScriptFormula(20);
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] += ScriptFormula(20);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				if (Rune_D > 0)     //Arcanot
				{
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 1] += ScriptFormula(32);
					Target.Attributes.BroadcastChangedIfRevealed();
				}

				return true;
			}

			public override void OnPayload(Payload payload)
			{
				//Ancient Guardian
				if (payload.Target == Target && payload is HitPayload && (payload as HitPayload).IsWeaponDamage)
					if (Rune_E > 0 && Target.Attributes[GameAttribute.Hitpoints_Cur] < Target.Attributes[GameAttribute.Hitpoints_Max_Total] * ScriptFormula(35))
						if (!HasBuff<AbsorbCDBuff>(payload.Target))
						{
							(payload as HitPayload).TotalDamage = 0;
							AddBuff(Target, new AbsorbCDBuff());
						}
			}

			const float _damageRate = 1f;
			TickTimer _damageTimer = null;

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);

					var closest_target = GetEnemiesInRadius(Target.Position, 35f).FilterByType<Monster>().GetClosestTo(Target.Position);
					if (closest_target != null)
					{
						var proj = new Projectile(this, RuneSelect(117557, 167807, 167978, 167814, 117557, 167817), Target.Position);
						proj.Position.Z += 5f;
						proj.OnCollision = (hit) =>
						{
							if (Rune_B > 0)     //Canoneer
								WeaponDamage(GetEnemiesInRadius(hit.Position, ScriptFormula(24)), ScriptFormula(2), DamageType.Arcane);
							else
								WeaponDamage(hit, ScriptFormula(2), Rune_A > 0 ? DamageType.Fire : (Rune_C > 0 ? DamageType.Cold : DamageType.Arcane));

							if (Rune_C > 0 && !HasBuff<DebuffFrozen>(hit))      //Icicle
								if (Rand.NextDouble() < ScriptFormula(27))
									AddBuff(hit, new DebuffFrozen(WaitSeconds(ScriptFormula(28))));

							proj.Destroy();
						};
						proj.Launch(closest_target.Position, 1f);
					}
				}
				return false;
			}

			public override void Remove()
			{
				base.Remove();

				if (Rune_A > 0)
				{
					Target.Attributes[GameAttribute.Amplify_Damage_Percent] -= ScriptFormula(20);
					Target.Attributes[GameAttribute.Damage_Percent_All_From_Skills] -= ScriptFormula(20);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				if (Rune_D > 0)
				{
					Target.Attributes[GameAttribute.Resource_Regen_Per_Second, 1] -= ScriptFormula(32);
					Target.Attributes.BroadcastChangedIfRevealed();
				}
			}
		}

		[ImplementsPowerBuff(3)]
		class AbsorbCDBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(10));
			}
		}
	}
	#endregion

	//Black Hole
	#region Black Hole
	[ImplementsPowerSNO(SkillsSystem.Skills.Wizard.Utility.BlackHole)]
	public class WizardBlackHole : PowerScript
	{
		public override IEnumerable<TickTimer> Run()
		{
			UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

			TargetPosition = PowerMath.TranslateDirection2D(User.Position, TargetPosition, User.Position, Math.Min(PowerMath.Distance2D(User.Position, TargetPosition), 40f));
			if (!User.World.CheckLocationForFlag(TargetPosition, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				yield break;

			StartCooldown(EvalTag(PowerKeys.CooldownTime));
			// cast effect
			User.PlayEffectGroup(RuneSelect(345619, 345623, 345621, 345622, 345624, 345621));
			SpawnEffect(RuneSelect(337757, 341373, 341412, 341427, 341442, 341410), TargetPosition + new Vector3D(0f, 0f, 5f), 0, WaitSeconds(ScriptFormula(0)));

			var wormHole = SpawnEffect(RuneSelect(336410, 341381, 341411, 341426, 341441, 341396), TargetPosition, 0, WaitSeconds(ScriptFormula(0)));
			wormHole.UpdateDelay = 0.5f;
			wormHole.OnUpdate = () =>
			{
				AttackPayload attack = new AttackPayload(this);
				attack.Targets = GetEnemiesInRadius(wormHole.Position, ScriptFormula(1));
				attack.AddWeaponDamage(ScriptFormula(2) / 4f, RuneSelect(DamageType.Arcane, DamageType.Lightning, DamageType.Arcane, DamageType.Fire, DamageType.Arcane, DamageType.Cold));
				attack.OnHit = (hit) =>
				{
					if (PowerMath.Distance2D(hit.Target.Position, wormHole.Position) > 2f)
						if (!HasBuff<KnockbackBuff>(hit.Target) && !HasBuff<DirectedKnockbackBuff>(hit.Target))
							AddBuff(hit.Target, new DirectedKnockbackBuff(wormHole.Position, -5f));

					if (Rune_D > 0 && !HasBuff<SpellStealBuff>(hit.Target))     //SpellSteal, done in HitPayload
					{
						AddBuff(hit.Target, new SpellStealBuff(0.1f, WaitSeconds(5f)));
						AddBuff(User, new DamageBuff());
					}

					if (Rune_E > 0) AddBuff(User, new ColdBuff());      //Absolute Zero, done in HitPayload	

					AddBuff(hit.Target, new DebuffStunned(WaitSeconds(0.5f)));
				};
				attack.Apply();

				if (Rune_B > 0)     //Event Horizon
				{
					var projectiles = wormHole.GetObjectsInRange<Projectile>(ScriptFormula(1));
					foreach (var proj in projectiles)
						proj.Destroy();
				}
			};
			wormHole.OnTimeout = () =>
			{
				if (Rune_C > 0)     //Blazar
				{
					SpawnEffect(343300, wormHole.Position, 0f, WaitSeconds(1f));
					WeaponDamage(GetEnemiesInRadius(wormHole.Position, ScriptFormula(1)), ScriptFormula(10), DamageType.Fire);
				}
			};
			yield break;
		}

		[ImplementsPowerBuff(6)]
		public class SpellStealBuff : PowerBuff
		{
			float Percentage = 0f;
			public SpellStealBuff(float percentage, TickTimer timeout)
			{
				Percentage = percentage;
				Timeout = timeout;
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && payload.Context.User == Target && (payload as HitPayload).IsWeaponDamage)
					(payload as HitPayload).TotalDamage *= 1 - Percentage;
			}

			public override void Remove()
			{
				base.Remove();
			}
		}

		[ImplementsPowerBuff(5, true)]
		public class ColdBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(30));
				MaxStackCount = 10;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
			private void _AddAmp()
			{
			}
		}

		[ImplementsPowerBuff(3, true)]
		public class DamageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(ScriptFormula(17));
				MaxStackCount = 10;
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				return true;
			}
			public override bool Stack(Buff buff)
			{
				bool stacked = StackCount < MaxStackCount;

				base.Stack(buff);

				//if (stacked)
				//_AddAmp();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
			}
			private void _AddAmp()
			{
			}
		}
	}

	#endregion
}

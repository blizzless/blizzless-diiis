//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Misc;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Movement;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
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
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class PowerContext
	{
		public static readonly Logger Logger = LogManager.CreateLogger();

		private static ThreadLocal<Random> _threadRand = new ThreadLocal<Random>(() => new Random());
		public static Random Rand { get { return _threadRand.Value; } }

		public int PowerSNO;
		public World World;
		public Actor User;
		public Actor Target;

		public int DogsSummoned = 0;

		// helper variables
		private TickTimer _defaultEffectTimeout;

		public void SetDefaultEffectTimeout(TickTimer timeout)
		{
			_defaultEffectTimeout = timeout;
		}

		public TickTimer WaitSeconds(float seconds)
		{
			return new SecondsTickTimer(World.Game, seconds);
		}

		public TickTimer WaitTicks(int ticks)
		{
			return new RelativeTickTimer(World.Game, ticks);
		}

		public TickTimer WaitInfinite()
		{
			return new TickTimer(World.Game, int.MaxValue);
		}

		public void StartCooldown(TickTimer timeout)
		{
			AddBuff(User, new Implementations.CooldownBuff(PowerSNO, timeout));
		}
		public void StartCooldownCharges(TickTimer timeout)
		{
			AddBuff(User, new Implementations.CooldownChargesBuff(PowerSNO, timeout));
		}

		public void StartCooldown(float seconds)
		{
			seconds -= User.Attributes[GameAttribute.Power_Cooldown_Reduction, PowerSNO];
			seconds *= (1f - User.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All]);
			StartCooldown(WaitSeconds(seconds));
		}

		public void StartCooldownCharges(float seconds)
		{
			//seconds -= User.Attributes[GameAttribute.Power_Cooldown_Reduction, PowerSNO];
			//seconds *= (1f - User.Attributes[GameAttribute.Power_Cooldown_Reduction_Percent_All]);
			StartCooldownCharges(WaitSeconds(seconds));
		}


		public void StartDefaultCooldown()
		{
			float seconds = EvalTag(PowerKeys.CooldownTime);
			if (seconds > 0f)
				StartCooldown(seconds);
		}

		public float GetProcCoefficient()
		{
			//Bugged powers list
			switch (PowerSNO)
			{
				case 0x00007805: return 0f;     //Hydra (Wiz)
				case 0x00012303: return 0f;     //Storm Armor (Wiz)
				case 0x00012D39: return 0.08f;  //Energy Twister (Wiz)
			}
			TagMap tagmap = _FindTagMapWithKey(PowerKeys.ProcCoefficient);
			if (tagmap != null)
				return EvalTag(PowerKeys.ProcCoefficient);
			else
				return 1f;
		}

		public void GeneratePrimaryResource(float amount)
		{
			if (User is Player)
			{
				(User as Player).GeneratePrimaryResource(amount);
			}
		}

		public void UsePrimaryResource(float amount)
		{
			if (User is Player)
			{
				if (User.Attributes[GameAttribute.Free_Cast_All] != true)
					(User as Player).UsePrimaryResource(amount - User.Attributes[GameAttribute.Power_Resource_Reduction, PowerSNO]);
			}
		}

		public void GenerateSecondaryResource(float amount)
		{
			if (User is Player)
			{
				(User as Player).GenerateSecondaryResource(amount);
			}
		}

		public void UseSecondaryResource(float amount)
		{
			if (User is Player)
			{
				(User as Player).UseSecondaryResource(amount - User.Attributes[GameAttribute.Power_Resource_Reduction, PowerSNO]);
			}
		}

		public void WeaponDamage(Actor target, float damageMultiplier, DamageType damageType)
		{
			AttackPayload payload = new AttackPayload(this);
			payload.SetSingleTarget(target);
			payload.AddWeaponDamage(damageMultiplier, damageType);
			payload.Apply();
		}

		public void WeaponDamage(TargetList targets, float damageMultiplier, DamageType damageType)
		{
			AttackPayload payload = new AttackPayload(this);
			payload.Targets = targets;
			payload.AddWeaponDamage(damageMultiplier, damageType);
			payload.Apply();
		}

		public void Damage(Actor target, float minDamage, float damageDelta, DamageType damageType)
		{
			AttackPayload payload = new AttackPayload(this);
			payload.SetSingleTarget(target);
			payload.AddDamage(minDamage, damageDelta, damageType);
			payload.Apply();
		}

		public void Damage(TargetList targets, float minDamage, float damageDelta, DamageType damageType)
		{
			AttackPayload payload = new AttackPayload(this);
			payload.Targets = targets;
			payload.AddDamage(minDamage, damageDelta, damageType);
			payload.Apply();
		}

		public EffectActor SpawnEffect(ActorSno actorSNO, Vector3D position, float angle = 0, TickTimer timeout = null)
		{
			if (angle == -1)
				angle = (float)(Rand.NextDouble() * (Math.PI * 2));
			if (timeout == null)
			{
				if (_defaultEffectTimeout == null)
					_defaultEffectTimeout = new SecondsTickTimer(World.Game, 2f); // default timeout of 2 seconds for now

				timeout = _defaultEffectTimeout;
			}

			var actor = new EffectActor(this, actorSNO, position);
			actor.Timeout = timeout;
			actor.Spawn(angle);
			//187359
			return actor;
		}

		public EffectActor SpawnEffect(ActorSno actorSNO, Vector3D position, Actor facingTarget, TickTimer timeout = null)
		{
			float angle = (facingTarget != null) ? MovementHelpers.GetFacingAngle(User.Position, facingTarget.Position) : -1f;
			return SpawnEffect(actorSNO, position, angle, timeout);
		}

		public EffectActor SpawnEffect(ActorSno actorSNO, Vector3D position, Vector3D facingTarget, TickTimer timeout = null)
		{
			float angle = MovementHelpers.GetFacingAngle(User.Position, facingTarget);
			return SpawnEffect(actorSNO, position, angle, timeout);
		}

		public EffectActor SpawnProxy(Vector3D position, TickTimer timeout = null)
		{
			return SpawnEffect(ActorSno._generic_proxy_normal, position, 0, timeout);
		}

		public TargetList GetEnemiesInRadius(Vector3D center, float radius, int maxCount = -1)
		{
			return _GetTargetsInRadiusHelper(center, radius, maxCount, (actor) => !actor.Dead, _EnemyActorFilter);
		}

		public TargetList GetAlliesInRadius(Vector3D center, float radius, int maxCount = -1)
		{
			return _GetTargetsInRadiusHelper(center, radius, maxCount, (actor) => !actor.Dead, _AllyActorFilter);
		}

		public TargetList GetEnemiesInBeamDirection(Vector3D startPoint, Vector3D direction,
													float length, float thickness = 0f)
		{
			Vector3D beamEnd = PowerMath.TranslateDirection2D(startPoint, direction, startPoint, length);

			return _GetTargetsInRadiusHelper(startPoint, length + thickness, -1,
				actor => PowerMath.CircleInBeam(new Circle(actor.Position.X, actor.Position.Y, actor.ActorData.Cylinder.Ax2),
												startPoint, beamEnd, thickness),
				_EnemyActorFilter);
		}

		public TargetList GetEnemiesInArcDirection(Vector3D center, Vector3D direction, float radius, float lengthDegrees)
		{
			Vector2F arcCenter2D = PowerMath.VectorWithoutZ(center);
			Vector2F arcDirection2D = PowerMath.VectorWithoutZ(direction);
			float arcLength = lengthDegrees * PowerMath.DegreesToRadians;

			return _GetTargetsInRadiusHelper(center, radius, -1,
				actor => PowerMath.ArcCircleCollides(arcCenter2D, arcDirection2D, radius, arcLength,
													 new Circle(actor.Position.X, actor.Position.Y, actor.ActorData.Cylinder.Ax2)),
				_EnemyActorFilter);
		}

		private TargetList _GetTargetsInRadiusHelper(Vector3D center, float radius, int maxCount,
			Func<Actor, bool> filter, Func<Actor, bool> targetFilter)
		{
			// actor radius currently used.
			TargetList targets = new TargetList();
			int count = 0;
			float radiusCompensation = 5f;
			foreach (Actor actor in World.QuadTree.Query<Actor>(new Circle(center.X, center.Y, radius + radiusCompensation + 25f)))
			{
				if (filter(actor) && !actor.Attributes[GameAttribute.Untargetable] && !World.PowerManager.IsDeletingActor(actor) && actor != User && (PowerMath.Distance2D(center, actor.Position) - (actor.ActorData.Cylinder.Ax2 + 5f)) <= radius + radiusCompensation)
				{
					if (targetFilter(actor))
					{
						if (count != maxCount)
						{
							targets.Actors.Add(actor);
							count += 1;
						}
					}
					else
					{
						targets.ExtraActors.Add(actor);
					}
				}
			}

			return targets;
		}

		private Func<Actor, bool> _EnemyActorFilter
		{
			get
			{
				if (World.IsPvP)
					return (actor) => ((actor is Player && actor.GlobalID != User.GlobalID) || (actor is Minion && actor.GlobalID != User.GlobalID && (actor as Minion).Master.GlobalID != User.GlobalID));
				else
				{
					if (User is Player || User is Minion || User is Hireling || (User is Monster && User.Attributes[GameAttribute.Team_Override] == 1) || User.SNO == ActorSno._pt_blacksmith_nonvendor || (User is Monster && User.Attributes[GameAttribute.Team_Override] == 1))
						return (actor) => (actor is Monster || actor is DesctructibleLootContainer) && actor.Visible && !(actor is ActorSystem.Implementations.ScriptObjects.ButcherFloorPanel) && !(actor is ActorSystem.Implementations.ScriptObjects.LeorFireGrate);
					else if (User is TownLeah || User is CaptainRumford || User is ArrowGuardian || User is LorathNahr_NPC)
						return (actor) => actor is Monster && actor.Visible;
					else if (User is CathedralLamp || User is ProximityTriggeredGizmo)
						return (actor) => (actor is Player || actor is Minion || actor is Hireling || actor is Monster) && actor.Visible;
					else
						return (actor) => (actor is DesctructibleLootContainer || actor is Player || actor is Minion || actor is Hireling || actor is LorathNahr_NPC) && actor.Visible;
				}
			}
		}

		private Func<Actor, bool> _AllyActorFilter
		{
			get
			{
				if (World.IsPvP)
					return (actor) => ((actor is Player || actor is Minion) && actor.GlobalID == User.GlobalID);
				else
				{
					if (User is Player || User is Minion || User is Hireling)
						return (actor) => (actor is Player || actor is Minion || actor is Hireling) && actor.Visible;
					else
						return (actor) => actor is Monster && actor.Visible;
				}
			}
		}

		public void TranslateEffect(Actor actor, Vector3D destination, float speed)
		{
			actor.Position = destination;

			if (actor.World == null) return;

			actor.World.BroadcastIfRevealed(plr => new ACDTranslateNormalMessage
			{
				ActorId = actor.DynamicID(plr),
				Position = destination,
				Angle = (float)Math.Acos(actor.RotationW) * 2f,  // convert z-axis quat to radians
				SnapFacing = true,
				MovementSpeed = speed,
			}, actor);
		}

		public TickTimer Knockback(Actor target, float magnitude, float arcHeight = 3.0f, float arcGravity = -0.03f)
		{
			if (target is Monster || target is Player || target is Minion || target is Hireling)
			{
				var buff = new Implementations.KnockbackBuff(magnitude, arcHeight, arcGravity);
				AddBuff(target, buff);
				return buff.ArrivalTime;
			}
			else
				return null;
		}

		public TickTimer Knockback(Vector3D from, Actor target, float magnitude, float arcHeight = 3.0f, float arcGravity = -0.03f)
		{
			if (target is Monster || target is Player || target is Minion || target is Hireling)
			{
				var buff = new Implementations.KnockbackBuff(magnitude, arcHeight, arcGravity);
				AddBuff(SpawnProxy(from), target, buff);
				return buff.ArrivalTime;
			}
			else
				return null;
		}

		public static bool ValidTarget(Actor target)
		{
			return target != null && target.World != null;
		}

		public bool ValidTarget()
		{
			return ValidTarget(Target);
		}

		public float ScriptFormula(int index)
		{
			float result;
			if (!ScriptFormulaEvaluator.Evaluate(PowerSNO, PowerTagHelper.GenerateTagForScriptFormula(index),
											 User.Attributes, Rand, out result))
			{
				Logger.Warn("Not Found ScriptFormula({0}) for power {1}", index, PowerSNO);
				return 0;
			}

			return result;
		}

		public int EvalTag(TagKeyInt key)
		{
			TagMap tagmap = _FindTagMapWithKey(key);
			if (tagmap != null)
				return tagmap[key];
			else
				return -1;
		}

		public int EvalTag(TagKeySNO key)
		{
			TagMap tagmap = _FindTagMapWithKey(key);
			if (tagmap != null)
				return tagmap[key].Id;
			else
				return -1;
		}

		public float EvalTag(TagKeyFloat key)
		{
			TagMap tagmap = _FindTagMapWithKey(key);
			if (tagmap != null)
				return tagmap[key];
			else
				return 0;
		}

		public float EvalTag(TagKeyScript key)
		{
			float result;
			if (!ScriptFormulaEvaluator.Evaluate(PowerSNO, key,
											 User.Attributes, Rand, out result))
				return 0;

			return result;
		}

		private TagMap _FindTagMapWithKey(TagKey key)
		{
			TagMap tagmap = PowerTagHelper.FindTagMapWithKey(PowerSNO, key);
			if (tagmap != null)
				return tagmap;
			else
			{
				//Logger.Error("could not find tag key {0} in power {1}", key.ID, PowerSNO);
				return null;
			}
		}

		public int Rune_A { get { return User.Attributes[GameAttribute.Rune_A, PowerSNO]; } }
		public int Rune_B { get { return User.Attributes[GameAttribute.Rune_B, PowerSNO]; } }
		public int Rune_C { get { return User.Attributes[GameAttribute.Rune_C, PowerSNO]; } }
		public int Rune_D { get { return User.Attributes[GameAttribute.Rune_D, PowerSNO]; } }
		public int Rune_E { get { return User.Attributes[GameAttribute.Rune_E, PowerSNO]; } }

		public T RuneSelect<T>(T none, T runeA, T runeB, T runeC, T runeD, T runeE)
		{
			if (Rune_A > 0) return runeA;
			else if (Rune_B > 0) return runeB;
			else if (Rune_C > 0) return runeC;
			else if (Rune_D > 0) return runeD;
			else if (Rune_E > 0) return runeE;
			else return none;
		}

		public bool AddBuff(Actor target, Buff buff)
		{
			return AddBuff(User, target, buff);
		}

		public bool AddBuff(Actor user, Actor target, Buff buff)
		{
			if (target is Monster || target is Player || target is Minion)
				return target.World.BuffManager.AddBuff(user, target, buff);
			else
				return false;
		}

		public void RemoveBuffs(Actor target, int PowerSNO)
		{
			if (target is Monster || target is Player || target is Minion)
				target.World.BuffManager.RemoveBuffs(target, PowerSNO);
		}

		public bool HasBuff<BuffType>(Actor target) where BuffType : Buff
		{
			return target.World.BuffManager.HasBuff<BuffType>(target);
		}

		public static Vector3D RandomDirection(Vector3D position, float radius)
		{
			return RandomDirection(position, radius, radius);
		}

		public static Vector3D RandomDirection(Vector3D position, float minRadius, float maxRadius)
		{
			float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
			float radius = minRadius + (float)FastRandom.Instance.NextDouble() * (maxRadius - minRadius);
			return new Vector3D(position.X + (float)Math.Cos(angle) * radius,
								position.Y + (float)Math.Sin(angle) * radius,
								position.Z);
		}
	}
}

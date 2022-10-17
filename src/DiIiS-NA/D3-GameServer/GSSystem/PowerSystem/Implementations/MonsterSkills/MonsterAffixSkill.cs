//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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
	public abstract class MonsterAffixSkill : ActionTimedSkill
	{
		public float CooldownTime = 10f;
	}

	[ImplementsPowerSNO(231115)]//PlagueCast
	public class MonsterAffixPlagued : MonsterAffixSkill
	{
		public new float CooldownTime = 5f;

		public override IEnumerable<TickTimer> Main()
		{
			var GroundSpot = SpawnProxy(User.Position);
			var plague = SpawnEffect(ActorSno._monsteraffix_plagued_endcloud, User.Position, 0, WaitSeconds(15f));
			plague.UpdateDelay = 1f;
			plague.OnUpdate = () =>
			{
				WeaponDamage(GetEnemiesInRadius(GroundSpot.Position, 3f), 0.2f, DamageType.Poison);
			};
			yield break;
		}
	}

	[ImplementsPowerSNO(231149)]//FrozenCast
	public class MonsterAffixFrozen : MonsterAffixSkill
	{
		public new float CooldownTime = 15f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				var GroundSpot = SpawnProxy(Target.Position);
				var frostCluster = SpawnEffect(ActorSno._monsteraffix_frozen_iceclusters, GroundSpot.Position, 0, WaitSeconds(4f));
				frostCluster.UpdateDelay = 1f;
				frostCluster.OnUpdate = () =>
				{
					WeaponDamage(GetEnemiesInRadius(GroundSpot.Position, 5f), 0.1f, DamageType.Cold);
				};
				frostCluster.OnTimeout = () =>
				{
					SpawnEffect(ActorSno._monsteraffix_frozen_deathexplosion_proxy, GroundSpot.Position);
					WeaponDamage(GetEnemiesInRadius(GroundSpot.Position, 7f), 1.5f, DamageType.Cold);
				};
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(155959)]//TeleportCast
	public class MonsterAffixTeleporter : MonsterAffixSkill
	{
		public new float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				User.PlayEffectGroup(170232);
				User.Teleport(RandomDirection(Target.Position, 10f, 30f));
				User.Unstuck();
				User.PlayEffectGroup(170232);
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(120305)]//VortexCast
	public class MonsterAffixVortex : MonsterAffixSkill
	{
		public new float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				Target.PlayEffectGroup(170232);
				Target.Teleport(RandomDirection(User.Position, 7f, 10f));
				Target.Unstuck();
				Target.PlayEffectGroup(170232);
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(156105)]
	public class MonsterAffixDesecrator : MonsterAffixSkill
	{
		public new float CooldownTime = 3f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				var GroundSpot = SpawnProxy(Target.Position);
				var aoe = SpawnEffect(ActorSno._monsteraffix_desecrator_damage_aoe, Target.Position, 0, WaitSeconds(12f));
				aoe.UpdateDelay = 1f;
				aoe.OnUpdate = () =>
				{
					WeaponDamage(GetEnemiesInRadius(GroundSpot.Position, 3f), 0.3f, DamageType.Physical);
				};
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(226294)]
	public class MonsterAffixWaller : MonsterAffixSkill
	{
		public new float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				SpawnEffect(ActorSno._monsteraffix_waller_model, RandomDirection(Target.Position, 5f, 15f), -1, WaitSeconds(10f));
			}
			yield break;
		}
	}

	[ImplementsPowerSNO(70650)]
	public class MonsterAffixExtraHealth : MonsterAffixSkill
	{
		public new float CooldownTime = 30f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new ExtraHealthBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ExtraHealthBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(30f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Hitpoints_Max] *= 2f;
				if (Target.Attributes[GameAttribute.Hitpoints_Max] / 2f == Target.Attributes[GameAttribute.Hitpoints_Cur])
					Target.Attributes[GameAttribute.Hitpoints_Cur] = Target.Attributes[GameAttribute.Hitpoints_Max];
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Hitpoints_Max] /= 2f;
				if (Target.Attributes[GameAttribute.Hitpoints_Max] < Target.Attributes[GameAttribute.Hitpoints_Cur])
					Target.Attributes[GameAttribute.Hitpoints_Cur] = Target.Attributes[GameAttribute.Hitpoints_Max];
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(226438)]
	public class MonsterAffixShielding : MonsterAffixSkill
	{
		public new float CooldownTime = 15f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new ShieldBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ShieldBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(5f);
			}
			public override void OnPayload(Payload payload)
			{
				if (payload is HitPayload && Target.Equals(payload.Target))
					(payload as HitPayload).TotalDamage *= 0.2f;
			}
		}
	}

	[ImplementsPowerSNO(70849)]
	public class MonsterAffixFast : MonsterAffixSkill
	{
		public new float CooldownTime = 30f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new FastBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class FastBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(30f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.WalkSpeed *= 1.5f;
				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.WalkSpeed /= 1.5f;
			}
		}
	}

	[ImplementsPowerSNO(222744)]
	public class MonsterAffixJailer : MonsterAffixSkill
	{
		public new float CooldownTime = 15f;

		public override IEnumerable<TickTimer> Main()
		{
			if (Target != null)
			{
				AddBuff(Target, new RootDebuff());
				AddBuff(Target, new RootCDDebuff());

			}
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class RootDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(3f);
			}
			public override bool Apply()
			{
				if (!base.Apply())
					return false;

				if (Target.Attributes[GameAttribute.Root_Immune] == false)
				{
					Target.Attributes[GameAttribute.IsRooted] = true;
					Target.Attributes.BroadcastChangedIfRevealed();
				}
				this.Target.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateSyncMessage() { ActorId = this.Target.DynamicID(plr), Position = this.Target.Position, Snap = false }, this.Target);

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.IsRooted] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}

		[ImplementsPowerBuff(1)]
		class RootCDDebuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(15f);
			}

			public override bool Apply()
			{
				if (!base.Apply())
					return false;
				Target.Attributes[GameAttribute.Root_Immune] = true;
				Target.Attributes.BroadcastChangedIfRevealed();

				return true;
			}
			public override void Remove()
			{
				base.Remove();
				Target.Attributes[GameAttribute.Root_Immune] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
		}
	}

	[ImplementsPowerSNO(81420)]
	public class MonsterAffixElectrified : MonsterAffixSkill
	{
		public new float CooldownTime = 10f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new LightningsBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class LightningsBuff : PowerBuff
		{
			const float _damageRate = 1f;
			TickTimer _damageTimer = null;
			public override void Init()
			{
				Timeout = WaitSeconds(10f);
			}

			public override bool Update()
			{
				if (base.Update())
					return true;
				if (_damageTimer == null || _damageTimer.TimedOut)
				{
					_damageTimer = WaitSeconds(_damageRate);
					for (int n = 0; n < 5; ++n)
					{
						var proj = new Projectile(this, ActorSno._wizard_chargedbolt_projectile, User.Position);
						proj.OnCollision = (hit) =>
						{
							SpawnEffect(ActorSno._wizard_chargedbolt_projectile, new Vector3D(hit.Position.X, hit.Position.Y, hit.Position.Z + 5f));
							WeaponDamage(hit, 0.2f, DamageType.Lightning);
							proj.Destroy();
						};
						proj.Launch(RandomDirection(User.Position, 18f, 20f), 0.3f);
						WaitSeconds(0.1f);
					}
				}
				return false;
			}
		}
	}

	[ImplementsPowerSNO(70655)]
	public class MonsterAffixKnockback : MonsterAffixSkill
	{
		public new float CooldownTime = 30f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new KnockbackTriggerBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class KnockbackTriggerBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(30f);
			}
			public override void OnPayload(Payload payload)
			{
				if (payload.Context.User == User && payload is HitPayload)
				{
					if (Rand.NextDouble() < 0.3f)
						Knockback(payload.Context.Target, 3f);
				}
			}
		}
	}

	[ImplementsPowerSNO(70655)]
	public class MonsterAffixReflectsDamage : MonsterAffixSkill
	{
		public new float CooldownTime = 30f;

		public override IEnumerable<TickTimer> Main()
		{
			AddBuff(User, new ReflectDamageBuff());
			yield break;
		}

		[ImplementsPowerBuff(0)]
		class ReflectDamageBuff : PowerBuff
		{
			public override void Init()
			{
				Timeout = WaitSeconds(30f);
			}
			public override void OnPayload(Payload payload)
			{
				if (payload.Target == User && payload is HitPayload)
				{
					if (Rune_A > 0)
					{
						Damage(payload.Context.User, (payload as HitPayload).TotalDamage * 0.1f, 0f, DamageType.Physical);
					}
				}
			}
		}
	}
}

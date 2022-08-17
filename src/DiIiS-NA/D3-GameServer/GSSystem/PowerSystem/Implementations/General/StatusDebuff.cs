//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
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
	public class SimpleBooleanStatusDebuff : PowerBuff
	{
		GameAttributeB _statusAttribute;
		GameAttributeB _immuneCheckAttribute;
		FloatingNumberMessage.FloatType? _floatMessage;
		bool _immuneBlocked;

		public SimpleBooleanStatusDebuff(GameAttributeB statusAttribute, GameAttributeB immuneCheckAttribute,
			FloatingNumberMessage.FloatType? floatMessage = null)
		{
			_statusAttribute = statusAttribute;
			_immuneCheckAttribute = immuneCheckAttribute;
			_floatMessage = floatMessage;
			_immuneBlocked = false;
		}

		public override void Init()
		{
			if (_immuneCheckAttribute != null)
				_immuneBlocked = Target.Attributes[_immuneCheckAttribute];
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			if (_immuneBlocked)
				return false;  // TODO: play immune float message?

			Target.Attributes[_statusAttribute] = true;
			Target.Attributes.BroadcastChangedIfRevealed();

			if (_floatMessage != null)
			{
				if (User is Player)
				{
					(User as Player).InGameClient.SendMessage(new FloatingNumberMessage
					{
						ActorID = this.Target.DynamicID(User as Player),
						Type = _floatMessage.Value
					});
				}
			}

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[_statusAttribute] = false;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		public override bool Stack(Buff buff)
		{
			if (((SimpleBooleanStatusDebuff)buff)._immuneBlocked)
				return true;  // swallow buff if it was blocked

			return base.Stack(buff);
		}
	}

	[ImplementsPowerSNO(103216)] // DebuffBlind.pow
	[ImplementsPowerBuff(0)]
	public class DebuffBlind : SimpleBooleanStatusDebuff
	{
		public DebuffBlind(TickTimer timeout)
			: base(GameAttribute.Blind, GameAttribute.Immune_To_Blind, FloatingNumberMessage.FloatType.Blinded)
		{
			Timeout = timeout;
		}
	}

	[ImplementsPowerSNO(473992)] // P6_Necro_Frailty_Aura.pow
	[ImplementsPowerBuff(0)]
	public class P6_Necro_Frailty_Aura : PowerBuff
	{
		public P6_Necro_Frailty_Aura()
			: base()
		{
			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][473992].Data;

		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			foreach (var enemy in User.GetMonstersInRange(15f))
				AddBuff(enemy, new FrailtyBuff());

			return false;
		}
	}
	[ImplementsPowerBuff(1)]
	public class FrailtyBuff : PowerBuff
	{
		const float _damageRate = 1f;
		TickTimer _damageTimer = null;

		public override void Init()
		{
			Timeout = WaitSeconds(2f);
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			return true;
		}
		public override void OnPayload(Payload payload)
		{
			if (payload.Target == Target && payload is HitPayload)
			{
				if (Target.Attributes[GameAttribute.Hitpoints_Cur] <= Target.Attributes[GameAttribute.Hitpoints_Max_Total] / 100 * 15)
				{
					Target.Attributes[GameAttribute.Hitpoints_Cur] = 0;
					Target.Attributes.BroadcastChangedIfRevealed();
					Remove();
				}
			}
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


	[ImplementsPowerSNO(474325)] // DebuffBlind.pow
	[ImplementsPowerBuff(0)]
	public class P6_Necro_Devour_Aura : PowerBuff
	{
		public P6_Necro_Devour_Aura()
			: base()
		{
			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][474325].Data;

		}
	}
	[ImplementsPowerBuff(1)]
	public class P6_Necro_Devour_Aura1 : PowerBuff
	{
		public P6_Necro_Devour_Aura1()
			: base()
		{

		}
	}
	[ImplementsPowerBuff(2)]
	public class P6_Necro_Devour_Aura2 : PowerBuff
	{
		public P6_Necro_Devour_Aura2()
			: base()
		{
			var PowerData = (DiIiS_NA.Core.MPQ.FileFormats.Power)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Power][474325].Data;

		}
	}

	[ImplementsPowerSNO(30195)] // DebuffChilled.pow
	[ImplementsPowerBuff(0)]
	public class DebuffChilled : SimpleBooleanStatusDebuff
	{
		public float Percentage;

		public DebuffChilled(float percentage, TickTimer timeout)
			: base(GameAttribute.Chilled, null, null)
		{
			Percentage = percentage;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;

			Target.WalkSpeed *= (1f - Percentage);
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();

			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
			}
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.WalkSpeed /= (1f - Percentage);
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(101000)] // DebuffStunned.pow
	[ImplementsPowerBuff(0)]
	public class DebuffStunned : SimpleBooleanStatusDebuff
	{
		public float Speed = 0;
		public DebuffStunned(TickTimer timeout)
			: base(GameAttribute.Stunned, GameAttribute.Stun_Immune, FloatingNumberMessage.FloatType.Stunned)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;


			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
				if ((Target as Player).SkillSet.HasPassive(209813)) //Provocation (Monk)
					AddBuff(Target, new ProvocationBuff());
			}

			if (Target is Boss)
			{
				(Target as Boss).AntiCCTriggerCount++;
				if ((Target as Boss).AntiCCTriggerCount >= 5)
				{
					AddBuff(Target, new AntiStun(WaitSeconds(10f)));
					(Target as Boss).AntiCCTriggerCount = 0;
				}
			}
			Speed = Target.WalkSpeed;
			Target.WalkSpeed = 0f;
			this.Target.World.BroadcastIfRevealed(plr => new MessageSystem.Message.Definitions.ACD.ACDTranslateSyncMessage() { ActorId = this.Target.DynamicID(plr), Position = this.Target.Position, Snap = false }, this.Target);

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.WalkSpeed = Speed;
		}
	}

	[ImplementsPowerSNO(30286)] // ImmuneToStunDuringBuff.pow
	[ImplementsPowerBuff(0)]
	public class AntiStun : SimpleBooleanStatusDebuff
	{
		public AntiStun(TickTimer timeout)
			: base(GameAttribute.Stun_Immune, null, FloatingNumberMessage.FloatType.BrokeStun)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			if (HasBuff<DebuffStunned>(Target))
				RemoveBuffs(Target, 101000);

			return true;
		}
	}

	[ImplementsPowerSNO(101002)] // DebuffFeared.pow
	[ImplementsPowerBuff(0)]
	public class DebuffFeared : SimpleBooleanStatusDebuff
	{
		public DebuffFeared(TickTimer timeout)
			: base(GameAttribute.Feared, GameAttribute.Fear_Immune, FloatingNumberMessage.FloatType.Feared)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;

			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
				if ((Target as Player).SkillSet.HasPassive(209813)) //Provocation (Monk)
					AddBuff(Target, new ProvocationBuff());
			}
			return true;
		}
	}

	[ImplementsPowerSNO(101003)] // DebuffRooted.pow
	[ImplementsPowerBuff(0)]
	public class DebuffRooted : SimpleBooleanStatusDebuff
	{
		public DebuffRooted(TickTimer timeout)
			: base(GameAttribute.IsRooted, GameAttribute.Root_Immune, FloatingNumberMessage.FloatType.Rooted)
		{
			Timeout = timeout;
		}
		//Seems there is no Rooted attribute.. so Stunned does the same thing.
		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;
			Target.Attributes[GameAttribute.Stunned] = true;
			Target.Attributes.BroadcastChangedIfRevealed();

			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
				if ((Target as Player).SkillSet.HasPassive(209813)) //Provocation (Monk)
					AddBuff(Target, new ProvocationBuff());
			}
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttribute.Stunned] = false;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}


	[ImplementsPowerSNO(100971)] // DebuffSlowed.pow
	[ImplementsPowerBuff(0)]
	public class DebuffSlowed : SimpleBooleanStatusDebuff
	{
		public float Percentage;

		public DebuffSlowed(float percentage, TickTimer timeout)
			: base(GameAttribute.Slow, GameAttribute.Slowdown_Immune, FloatingNumberMessage.FloatType.Snared)
		{
			Percentage = percentage;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;
			Target.WalkSpeed *= (1f - Percentage);
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();

			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
			}
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.WalkSpeed /= (1f - Percentage);
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(60777)] // Generic_Taunt.pow
	[ImplementsPowerBuff(0)]
	public class DebuffTaunted : PowerBuff
	{
		public DebuffTaunted(TickTimer timeout)
		{
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;

			try
			{
				if (Target is Monster)
				{
					((Target as Monster).Brain as MonsterBrain).PriorityTarget = User;
				}
			}
			catch { }
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			try
			{
				if (Target is Monster)
				{
					((Target as Monster).Brain as MonsterBrain).PriorityTarget = null;
				}
			}
			catch { }
		}
	}

	[ImplementsPowerBuff(0)]
	public class DebuffFrozen : SimpleBooleanStatusDebuff
	{
		public DebuffFrozen(TickTimer timeout)
			: base(GameAttribute.Frozen, GameAttribute.Freeze_Immune, FloatingNumberMessage.FloatType.Frozen)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;

			if (Target is Player)
			{
				if ((Target as Player).SkillSet.HasPassive(205707)) //Juggernaut (barbarian)
					if (FastRandom.Instance.Next(100) < 30)
						(Target as Player).AddPercentageHP(20);
				if ((Target as Player).SkillSet.HasPassive(209813)) //Provocation (Monk)
					AddBuff(Target, new ProvocationBuff());
			}
			return true;
		}
	}
	//----------------------------------------------------------------------------------------------
	//These are Skill-Related Powers
	[ImplementsPowerSNO(1755)] // SlowTimeDebuff.pow
	[ImplementsPowerBuff(0)]
	public class SlowTimeDebuff : SimpleBooleanStatusDebuff
	{
		public float Percentage;

		public SlowTimeDebuff(float percentage, TickTimer timeout)
			: base(GameAttribute.Slow, GameAttribute.Slowdown_Immune, FloatingNumberMessage.FloatType.Snared)
		{
			Percentage = percentage;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply() || Target.Attributes[GameAttribute.Immunity] == true)
				return false;
			//is my projectile speed correct?
			Target.WalkSpeed *= (1f - Percentage);
			Target.Attributes[GameAttribute.Projectile_Speed] += Target.Attributes[GameAttribute.Projectile_Speed] * 0.1f;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] += Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.WalkSpeed /= (1f - Percentage);
			Target.Attributes[GameAttribute.Projectile_Speed] += Target.Attributes[GameAttribute.Projectile_Speed] / 0.1f;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += Percentage;
			Target.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	//Wrong Section but i'm just gonna put it here.
	[ImplementsPowerSNO(218385)]
	[ImplementsPowerBuff(0)]
	public class MovementBuff : PowerBuff
	{
		public float Percentage;

		public MovementBuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
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

	[ImplementsPowerSNO(1769)]
	[ImplementsPowerBuff(3)]
	public class SpeedBuff : PowerBuff
	{
		public float Percentage;

		public SpeedBuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttribute.Casting_Speed_Percent] += Percentage;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] += Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttribute.Casting_Speed_Percent] -= Percentage;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Percent] -= Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(211581)]
	[ImplementsPowerBuff(0)]
	public class DamageReduceDebuff : PowerBuff
	{
		public float Percentage;

		public DamageReduceDebuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Context.User == Target)
				(payload as HitPayload).TotalDamage *= 1 - Percentage;
		}
	}

	[ImplementsPowerSNO(156492)]
	[ImplementsPowerBuff(1)]
	public class GuidingLightBuff : PowerBuff
	{
		public float Percentage = 0f;

		public GuidingLightBuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Context.User == Target && (payload as HitPayload).IsWeaponDamage)
				(payload as HitPayload).TotalDamage *= 1 + Percentage;
		}
	}

	[ImplementsPowerSNO(218044)]
	[ImplementsPowerBuff(0)]
	public class ArmorReduceDebuff : PowerBuff
	{
		public float Percentage;

		public ArmorReduceDebuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
		}

		public override void OnPayload(Payload payload)
		{
			if (payload is HitPayload && payload.Target == Target)
				(payload as HitPayload).TotalDamage *= 1 + Percentage;
		}
	}

	[ImplementsPowerSNO(285903)]
	[ImplementsPowerBuff(1)]
	public class RebirthHitPointRegenBuff : PowerBuff       //Crusader Punish -> Rebirth
	{
		public float Regen;

		public RebirthHitPointRegenBuff(float regen, TickTimer timeout)
		{
			Regen = regen;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] += Regen;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttribute.Hitpoints_Regen_Per_Second_Bonus] -= Regen;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(285903)]
	[ImplementsPowerBuff(3)]
	public class CeleritySpeedBuff : PowerBuff      //Crusader Punish -> Celerity
	{
		public float Percentage;

		public CeleritySpeedBuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
		}
		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttribute.Casting_Speed_Bonus] += Percentage;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] += Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttribute.Casting_Speed_Bonus] -= Percentage;
			Target.Attributes[GameAttribute.Attacks_Per_Second_Bonus] -= Percentage;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(291804)]
	[ImplementsPowerBuff(3)]
	public class FlashSpeedBuff : PowerBuff     //Crusader IronSkin -> Flash
	{
		public float Percentage;

		public FlashSpeedBuff(float percentage, TickTimer timeout)
		{
			Percentage = percentage;
			Timeout = timeout;
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
}

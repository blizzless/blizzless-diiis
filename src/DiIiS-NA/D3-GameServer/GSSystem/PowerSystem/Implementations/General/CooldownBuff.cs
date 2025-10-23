using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(30176)]  // Cooldown.pow
	[ImplementsPowerBuff(0)]
	public class CooldownBuff : PowerBuff
	{
		public int TargetPowerSNO;
		private float? _seconds;

		public CooldownBuff(int targetPowerSNO, TickTimer timeout)
		{
			TargetPowerSNO = targetPowerSNO;
			Timeout = timeout;
		}

		public CooldownBuff(int targetPowerSNO, float seconds)
		{
			TargetPowerSNO = targetPowerSNO;
			_seconds = seconds;
		}

		public override void Init()
		{
			base.Init();
			if (_seconds.HasValue)
				Timeout = WaitSeconds(_seconds.Value);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			Target.Attributes[GameAttributes.Power_Cooldown_Start, TargetPowerSNO] = World.Game.TickCounter;
			Target.Attributes[GameAttributes.Power_Cooldown, TargetPowerSNO] = Timeout.TimeoutTick;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Power_Cooldown_Start, TargetPowerSNO] = 0;
			Target.Attributes[GameAttributes.Power_Cooldown, TargetPowerSNO] = 0;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		public override bool Stack(Buff buff)
		{
			// multiple cooldowns of different target powers are allowed
			// and multiple cooldowns on the same power should never happen
			return false;
		}
	}
	[ImplementsPowerBuff(1)]
	public class CooldownChargesBuff : PowerBuff
	{
		public int TargetPowerSNO;
		private float? _seconds;

		public CooldownChargesBuff(int targetPowerSNO, TickTimer timeout)
		{
			TargetPowerSNO = targetPowerSNO;
			Timeout = timeout;
		}

		public CooldownChargesBuff(int targetPowerSNO, float seconds)
		{
			TargetPowerSNO = targetPowerSNO;
			_seconds = seconds;
		}

		public override void Init()
		{
			base.Init();
			if (_seconds.HasValue)
				Timeout = WaitSeconds(_seconds.Value);
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			
			Target.Attributes[GameAttributes.Recharge_Start_Time, TargetPowerSNO] = World.Game.TickCounter;
			Target.Attributes[GameAttributes.Next_Charge_Gained_time, TargetPowerSNO] = Timeout.TimeoutTick;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Recharge_Start_Time, TargetPowerSNO] = 0;
			Target.Attributes[GameAttributes.Next_Charge_Gained_time, TargetPowerSNO] = 0;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		public override bool Stack(Buff buff)
		{
			// multiple cooldowns of different target powers are allowed
			// and multiple cooldowns on the same power should never happen
			return false;
		}
	}


	[ImplementsPowerSNO(129212)]//96719)]  // PVPSkirmishBuff.pow - replaced to Demon_Hunter_Preparation
	[ImplementsPowerBuff(0)]
	public class PVPSkirmishBuff : PowerBuff
	{

		public PVPSkirmishBuff(TickTimer timeout)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Disabled] = true;
			Target.Attributes[GameAttributes.Immobolize] = true;
			Target.Attributes[GameAttributes.Untargetable] = true;
			Target.Attributes[GameAttributes.CantStartDisplayedPowers] = true;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Disabled] = false;
			Target.Attributes[GameAttributes.Immobolize] = false;
			Target.Attributes[GameAttributes.Untargetable] = false;
			Target.Attributes[GameAttributes.CantStartDisplayedPowers] = false;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(97359)]  // PVPBuff.pow 
	[ImplementsPowerBuff(0)]
	public class PVPBuff : PowerBuff
	{

		public PVPBuff(TickTimer timeout)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			//Target.Attributes[GameAttribute.Disabled] = true;
			//Target.Attributes[GameAttribute.Immobolize] = true;
			//Target.Attributes[GameAttribute.Untargetable] = true;
			//Target.Attributes[GameAttribute.CantStartDisplayedPowers] = true;
			//Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			//Target.Attributes[GameAttribute.Disabled] = false;
			//Target.Attributes[GameAttribute.Immobolize] = false;
			//Target.Attributes[GameAttribute.Untargetable] = false;
			//Target.Attributes[GameAttribute.CantStartDisplayedPowers] = false;
			//Target.Attributes.BroadcastChangedIfRevealed();
		}
	}

	[ImplementsPowerSNO(265724)]// PVP_Spawner_Spawn 
	[ImplementsPowerBuff(0)]
	public class PVPSafeZoneBuff : PowerBuff
	{
		TickTimer _tickTimer = null;

		public PVPSafeZoneBuff()
		{
		}

		public override bool Update()
		{
			if (base.Update())
				return true;

			if (_tickTimer == null || _tickTimer.TimedOut)
			{
				foreach (Actor Ally in Target.GetActorsInRange(45f))
				{
					if (Ally is Minion || Ally is Player)
					{
						AddBuff(Ally, new PVPSafeBuff(WaitSeconds(1f)));
					}
				}
				_tickTimer = WaitSeconds(1f);
			}
			return false;
		}
	}

	[ImplementsPowerSNO(220304)]// ActorInTownBuff 
	[ImplementsPowerBuff(0)]
	public class PVPSafeBuff : PowerBuff
	{

		public PVPSafeBuff(TickTimer timeout)
		{
			Timeout = timeout;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;
			Target.Attributes[GameAttributes.Invulnerable] = true;
			Target.Attributes[GameAttributes.Disabled] = true;
			Target.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Invulnerable] = false;
			Target.Attributes[GameAttributes.Disabled] = false;
			Target.Attributes.BroadcastChangedIfRevealed();
		}
	}
}

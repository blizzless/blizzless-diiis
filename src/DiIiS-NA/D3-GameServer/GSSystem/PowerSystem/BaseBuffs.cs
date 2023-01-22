//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations;
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


namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class Buff : PowerContext
	{
		public bool Removed = false;

		public virtual bool Apply()
		{
			if (PowerSNO != 0)
			{
				Target.Attributes[GameAttribute.Buff_Exclusive_Type_Active, PowerSNO] = true;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			return true;
		}

		public virtual void Remove()
		{
			if (PowerSNO != 0)
			{
				Target.Attributes[GameAttribute.Buff_Exclusive_Type_Active, PowerSNO] = false;
				Target.Attributes.BroadcastChangedIfRevealed();
			}
			Removed = true;
		}

		public virtual void Init() { }
		public virtual bool Update() { return false; }
		public virtual bool Stack(Buff buff) { return false; }
		public virtual bool DeStack(Buff buff) { return false; }
		public virtual void OnPayload(Payloads.Payload payload) { }
	}

	public abstract class TimedBuff : Buff
	{
		public TickTimer Timeout;

		public override bool Update()
		{
			if (Target.World == null)
			{
				Remove();
				return true;
			}
			return (Timeout != null && Timeout.TimedOut) || Removed;
		}

		public override bool Stack(Buff buff)
		{
			TimedBuff newbuff = (TimedBuff)buff;
			// update buff if new timeout is longer than current one, or if new buff has no timeout
			if (newbuff.Timeout == null || Timeout != null && newbuff.Timeout.TimeoutTick > Timeout.TimeoutTick)
				Timeout = newbuff.Timeout;

			return true;
		}

		public override bool DeStack(Buff buff)
		{
			TimedBuff newbuff = (TimedBuff)buff;
			// update buff if new timeout is longer than current one, or if new buff has no timeout
			if (newbuff.Timeout == null || Timeout != null && newbuff.Timeout.TimeoutTick > Timeout.TimeoutTick)
				Timeout = newbuff.Timeout;

			return true;
		}
	}

	public abstract class PowerBuff : TimedBuff
	{
		public int BuffSlot = 0;
		public bool IsPersistent = false;
		public bool IsCountingStacks = false;
		public int StackCount = 0;
		public int MaxStackCount = 0;
		private int StartTick = 0;

		public PowerBuff()
		{
			// try to load buff options from attribute
			var attributes = (ImplementsPowerBuff[])GetType().GetCustomAttributes(typeof(ImplementsPowerBuff), true);
			foreach (var attr in attributes)
			{
				BuffSlot = attr.BuffSlot;
				IsCountingStacks = attr.CountStacks;
			}
		}

		public override bool Apply()
		{
			base.Apply();

			if (StartTick == 0)
				StartTick = Target.World.Game.TickCounter;

			Target.Attributes[_Power_Buff_N_VisualEffect_R, PowerSNO] = true;
			if (Timeout != null)
			{
				Target.Attributes[_Buff_Icon_Start_TickN, PowerSNO] = StartTick;
				Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = Timeout.TimeoutTick;
				Target.Attributes[_Buff_Icon_CountN, PowerSNO] = 1;
			}
			Target.Attributes.BroadcastChangedIfRevealed();

			StackCount = 1;

			return true;
		}

		public override void Remove()
		{
			base.Remove();

			Target.Attributes[_Power_Buff_N_VisualEffect_R, PowerSNO] = false;
			if (Timeout != null)
			{
				Target.Attributes[_Buff_Icon_Start_TickN, PowerSNO] = 0;
				Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = 0;
				Target.Attributes[_Buff_Icon_CountN, PowerSNO] = 0;
			}
			Target.Attributes.BroadcastChangedIfRevealed();

			//this.StackCount = 0;
		}

		public override bool Stack(Buff buff)
		{
			base.Stack(buff);

			bool canStack = IsCountingStacks && StackCount != MaxStackCount;

			if (Timeout != null)
			{
				StartTick = Target.World.Game.TickCounter;
				Target.Attributes[_Buff_Icon_Start_TickN, PowerSNO] = StartTick;
				Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = Timeout.TimeoutTick;
				if (canStack)
					Target.Attributes[_Buff_Icon_CountN, PowerSNO] += 1;
			}
			Target.Attributes.BroadcastChangedIfRevealed();

			if (canStack)
				StackCount += 1;

			return true;
		}

		public override bool DeStack(Buff buff)
		{
			base.DeStack(buff);

			bool canDeStack = IsCountingStacks && StackCount > 1;

			if (Timeout != null)
			{
				Target.Attributes[_Buff_Icon_Start_TickN, PowerSNO] = Timeout.TimeoutTick;
				Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = Timeout.TimeoutTick;
				if (canDeStack)
					Target.Attributes[_Buff_Icon_CountN, PowerSNO] -= 1;
			}
			Target.Attributes.BroadcastChangedIfRevealed();

			if (canDeStack) StackCount -= 1;

			return true;
		}

		public void Extend(int ticks)
		{
			Timeout.TimeoutTick += ticks;
			Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = Timeout.TimeoutTick;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		public void Reduce(int ticks)
		{
			Timeout.TimeoutTick -= ticks;
			if (this is CooldownBuff)
			{
				Target.Attributes[GameAttribute.Power_Cooldown_Start, (this as CooldownBuff).TargetPowerSNO] -= ticks;
				Target.Attributes[GameAttribute.Power_Cooldown, (this as CooldownBuff).TargetPowerSNO] = Timeout.TimeoutTick;
			}
			Target.Attributes[_Buff_Icon_End_TickN, PowerSNO] = Timeout.TimeoutTick;
			Target.Attributes.BroadcastChangedIfRevealed();
		}

		private GameAttributeB _Power_Buff_N_VisualEffect_R
		{
			get
			{
				switch (BuffSlot)
				{
					default:
					case 0:
						return RuneSelect(GameAttribute.Power_Buff_0_Visual_Effect_None,
										  GameAttribute.Power_Buff_0_Visual_Effect_A,
										  GameAttribute.Power_Buff_0_Visual_Effect_B,
										  GameAttribute.Power_Buff_0_Visual_Effect_C,
										  GameAttribute.Power_Buff_0_Visual_Effect_D,
										  GameAttribute.Power_Buff_0_Visual_Effect_E);
					case 1:
						return RuneSelect(GameAttribute.Power_Buff_1_Visual_Effect_None,
										  GameAttribute.Power_Buff_1_Visual_Effect_A,
										  GameAttribute.Power_Buff_1_Visual_Effect_B,
										  GameAttribute.Power_Buff_1_Visual_Effect_C,
										  GameAttribute.Power_Buff_1_Visual_Effect_D,
										  GameAttribute.Power_Buff_1_Visual_Effect_E);
					case 2:
						return RuneSelect(GameAttribute.Power_Buff_2_Visual_Effect_None,
										  GameAttribute.Power_Buff_2_Visual_Effect_A,
										  GameAttribute.Power_Buff_2_Visual_Effect_B,
										  GameAttribute.Power_Buff_2_Visual_Effect_C,
										  GameAttribute.Power_Buff_2_Visual_Effect_D,
										  GameAttribute.Power_Buff_2_Visual_Effect_E);
					case 3:
						return RuneSelect(GameAttribute.Power_Buff_3_Visual_Effect_None,
										  GameAttribute.Power_Buff_3_Visual_Effect_A,
										  GameAttribute.Power_Buff_3_Visual_Effect_B,
										  GameAttribute.Power_Buff_3_Visual_Effect_C,
										  GameAttribute.Power_Buff_3_Visual_Effect_D,
										  GameAttribute.Power_Buff_3_Visual_Effect_E);
					case 4:
						return RuneSelect(GameAttribute.Power_Buff_4_Visual_Effect_None,
										  GameAttribute.Power_Buff_4_Visual_Effect_A,
										  GameAttribute.Power_Buff_4_Visual_Effect_B,
										  GameAttribute.Power_Buff_4_Visual_Effect_C,
										  GameAttribute.Power_Buff_4_Visual_Effect_D,
										  GameAttribute.Power_Buff_4_Visual_Effect_E);
					case 5:
						return RuneSelect(GameAttribute.Power_Buff_5_Visual_Effect_None,
										  GameAttribute.Power_Buff_5_Visual_Effect_A,
										  GameAttribute.Power_Buff_5_Visual_Effect_B,
										  GameAttribute.Power_Buff_5_Visual_Effect_C,
										  GameAttribute.Power_Buff_5_Visual_Effect_D,
										  GameAttribute.Power_Buff_5_Visual_Effect_E);
					case 6:
						return RuneSelect(GameAttribute.Power_Buff_6_Visual_Effect_None,
										  GameAttribute.Power_Buff_6_Visual_Effect_A,
										  GameAttribute.Power_Buff_6_Visual_Effect_B,
										  GameAttribute.Power_Buff_6_Visual_Effect_C,
										  GameAttribute.Power_Buff_6_Visual_Effect_D,
										  GameAttribute.Power_Buff_6_Visual_Effect_E);
					case 7:
						return RuneSelect(GameAttribute.Power_Buff_7_Visual_Effect_None,
										  GameAttribute.Power_Buff_7_Visual_Effect_A,
										  GameAttribute.Power_Buff_7_Visual_Effect_B,
										  GameAttribute.Power_Buff_7_Visual_Effect_C,
										  GameAttribute.Power_Buff_7_Visual_Effect_D,
										  GameAttribute.Power_Buff_7_Visual_Effect_E);
					case 8:
						return RuneSelect(GameAttribute.Power_Buff_8_Visual_Effect_None,
										  GameAttribute.Power_Buff_8_Visual_Effect_A,
										  GameAttribute.Power_Buff_8_Visual_Effect_B,
										  GameAttribute.Power_Buff_8_Visual_Effect_C,
										  GameAttribute.Power_Buff_8_Visual_Effect_D,
										  GameAttribute.Power_Buff_8_Visual_Effect_E);
					case 9:
						return RuneSelect(GameAttribute.Power_Buff_9_Visual_Effect_None,
										  GameAttribute.Power_Buff_9_Visual_Effect_A,
										  GameAttribute.Power_Buff_9_Visual_Effect_B,
										  GameAttribute.Power_Buff_9_Visual_Effect_C,
										  GameAttribute.Power_Buff_9_Visual_Effect_D,
										  GameAttribute.Power_Buff_9_Visual_Effect_E);
					case 10:
						return RuneSelect(GameAttribute.Power_Buff_10_Visual_Effect_None,
										  GameAttribute.Power_Buff_10_Visual_Effect_A,
										  GameAttribute.Power_Buff_10_Visual_Effect_B,
										  GameAttribute.Power_Buff_10_Visual_Effect_C,
										  GameAttribute.Power_Buff_10_Visual_Effect_D,
										  GameAttribute.Power_Buff_10_Visual_Effect_E);
					case 11:
						return RuneSelect(GameAttribute.Power_Buff_11_Visual_Effect_None,
										  GameAttribute.Power_Buff_11_Visual_Effect_A,
										  GameAttribute.Power_Buff_11_Visual_Effect_B,
										  GameAttribute.Power_Buff_11_Visual_Effect_C,
										  GameAttribute.Power_Buff_11_Visual_Effect_D,
										  GameAttribute.Power_Buff_11_Visual_Effect_E);
					case 12:
						return RuneSelect(GameAttribute.Power_Buff_12_Visual_Effect_None,
										  GameAttribute.Power_Buff_12_Visual_Effect_A,
										  GameAttribute.Power_Buff_12_Visual_Effect_B,
										  GameAttribute.Power_Buff_12_Visual_Effect_C,
										  GameAttribute.Power_Buff_12_Visual_Effect_D,
										  GameAttribute.Power_Buff_12_Visual_Effect_E);
					case 13:
						return RuneSelect(GameAttribute.Power_Buff_13_Visual_Effect_None,
										  GameAttribute.Power_Buff_13_Visual_Effect_A,
										  GameAttribute.Power_Buff_13_Visual_Effect_B,
										  GameAttribute.Power_Buff_13_Visual_Effect_C,
										  GameAttribute.Power_Buff_13_Visual_Effect_D,
										  GameAttribute.Power_Buff_13_Visual_Effect_E);
					case 14:
						return RuneSelect(GameAttribute.Power_Buff_14_Visual_Effect_None,
										  GameAttribute.Power_Buff_14_Visual_Effect_A,
										  GameAttribute.Power_Buff_14_Visual_Effect_B,
										  GameAttribute.Power_Buff_14_Visual_Effect_C,
										  GameAttribute.Power_Buff_14_Visual_Effect_D,
										  GameAttribute.Power_Buff_14_Visual_Effect_E);
					case 15:
						return RuneSelect(GameAttribute.Power_Buff_15_Visual_Effect_None,
										  GameAttribute.Power_Buff_15_Visual_Effect_A,
										  GameAttribute.Power_Buff_15_Visual_Effect_B,
										  GameAttribute.Power_Buff_15_Visual_Effect_C,
										  GameAttribute.Power_Buff_15_Visual_Effect_D,
										  GameAttribute.Power_Buff_15_Visual_Effect_E);
					case 16:
						return RuneSelect(GameAttribute.Power_Buff_16_Visual_Effect_None,
										  GameAttribute.Power_Buff_16_Visual_Effect_A,
										  GameAttribute.Power_Buff_16_Visual_Effect_B,
										  GameAttribute.Power_Buff_16_Visual_Effect_C,
										  GameAttribute.Power_Buff_16_Visual_Effect_D,
										  GameAttribute.Power_Buff_16_Visual_Effect_E);
					case 17:
						return RuneSelect(GameAttribute.Power_Buff_17_Visual_Effect_None,
										  GameAttribute.Power_Buff_17_Visual_Effect_A,
										  GameAttribute.Power_Buff_17_Visual_Effect_B,
										  GameAttribute.Power_Buff_17_Visual_Effect_C,
										  GameAttribute.Power_Buff_17_Visual_Effect_D,
										  GameAttribute.Power_Buff_17_Visual_Effect_E);
					case 18:
						return RuneSelect(GameAttribute.Power_Buff_18_Visual_Effect_None,
										  GameAttribute.Power_Buff_18_Visual_Effect_A,
										  GameAttribute.Power_Buff_18_Visual_Effect_B,
										  GameAttribute.Power_Buff_18_Visual_Effect_C,
										  GameAttribute.Power_Buff_18_Visual_Effect_D,
										  GameAttribute.Power_Buff_18_Visual_Effect_E);
					case 19:
						return RuneSelect(GameAttribute.Power_Buff_19_Visual_Effect_None,
										  GameAttribute.Power_Buff_19_Visual_Effect_A,
										  GameAttribute.Power_Buff_19_Visual_Effect_B,
										  GameAttribute.Power_Buff_19_Visual_Effect_C,
										  GameAttribute.Power_Buff_19_Visual_Effect_D,
										  GameAttribute.Power_Buff_19_Visual_Effect_E);
					case 20:
						return RuneSelect(GameAttribute.Power_Buff_20_Visual_Effect_None,
										  GameAttribute.Power_Buff_20_Visual_Effect_A,
										  GameAttribute.Power_Buff_20_Visual_Effect_B,
										  GameAttribute.Power_Buff_20_Visual_Effect_C,
										  GameAttribute.Power_Buff_20_Visual_Effect_D,
										  GameAttribute.Power_Buff_20_Visual_Effect_E);
					case 21:
						return RuneSelect(GameAttribute.Power_Buff_21_Visual_Effect_None,
										  GameAttribute.Power_Buff_21_Visual_Effect_A,
										  GameAttribute.Power_Buff_21_Visual_Effect_B,
										  GameAttribute.Power_Buff_21_Visual_Effect_C,
										  GameAttribute.Power_Buff_21_Visual_Effect_D,
										  GameAttribute.Power_Buff_21_Visual_Effect_E);
					case 22:
						return RuneSelect(GameAttribute.Power_Buff_22_Visual_Effect_None,
										  GameAttribute.Power_Buff_22_Visual_Effect_A,
										  GameAttribute.Power_Buff_22_Visual_Effect_B,
										  GameAttribute.Power_Buff_22_Visual_Effect_C,
										  GameAttribute.Power_Buff_22_Visual_Effect_D,
										  GameAttribute.Power_Buff_22_Visual_Effect_E);
					case 23:
						return RuneSelect(GameAttribute.Power_Buff_23_Visual_Effect_None,
										  GameAttribute.Power_Buff_23_Visual_Effect_A,
										  GameAttribute.Power_Buff_23_Visual_Effect_B,
										  GameAttribute.Power_Buff_23_Visual_Effect_C,
										  GameAttribute.Power_Buff_23_Visual_Effect_D,
										  GameAttribute.Power_Buff_23_Visual_Effect_E);
					case 24:
						return RuneSelect(GameAttribute.Power_Buff_24_Visual_Effect_None,
										  GameAttribute.Power_Buff_24_Visual_Effect_A,
										  GameAttribute.Power_Buff_24_Visual_Effect_B,
										  GameAttribute.Power_Buff_24_Visual_Effect_C,
										  GameAttribute.Power_Buff_24_Visual_Effect_D,
										  GameAttribute.Power_Buff_24_Visual_Effect_E);
					case 25:
						return RuneSelect(GameAttribute.Power_Buff_25_Visual_Effect_None,
										  GameAttribute.Power_Buff_25_Visual_Effect_A,
										  GameAttribute.Power_Buff_25_Visual_Effect_B,
										  GameAttribute.Power_Buff_25_Visual_Effect_C,
										  GameAttribute.Power_Buff_25_Visual_Effect_D,
										  GameAttribute.Power_Buff_25_Visual_Effect_E);
					case 26:
						return RuneSelect(GameAttribute.Power_Buff_26_Visual_Effect_None,
										  GameAttribute.Power_Buff_26_Visual_Effect_A,
										  GameAttribute.Power_Buff_26_Visual_Effect_B,
										  GameAttribute.Power_Buff_26_Visual_Effect_C,
										  GameAttribute.Power_Buff_26_Visual_Effect_D,
										  GameAttribute.Power_Buff_26_Visual_Effect_E);
					case 27:
						return RuneSelect(GameAttribute.Power_Buff_27_Visual_Effect_None,
										  GameAttribute.Power_Buff_27_Visual_Effect_A,
										  GameAttribute.Power_Buff_27_Visual_Effect_B,
										  GameAttribute.Power_Buff_27_Visual_Effect_C,
										  GameAttribute.Power_Buff_27_Visual_Effect_D,
										  GameAttribute.Power_Buff_27_Visual_Effect_E);
					case 28:
						return RuneSelect(GameAttribute.Power_Buff_28_Visual_Effect_None,
										  GameAttribute.Power_Buff_28_Visual_Effect_A,
										  GameAttribute.Power_Buff_28_Visual_Effect_B,
										  GameAttribute.Power_Buff_28_Visual_Effect_C,
										  GameAttribute.Power_Buff_28_Visual_Effect_D,
										  GameAttribute.Power_Buff_28_Visual_Effect_E);
					case 29:
						return RuneSelect(GameAttribute.Power_Buff_29_Visual_Effect_None,
										  GameAttribute.Power_Buff_29_Visual_Effect_A,
										  GameAttribute.Power_Buff_29_Visual_Effect_B,
										  GameAttribute.Power_Buff_29_Visual_Effect_C,
										  GameAttribute.Power_Buff_29_Visual_Effect_D,
										  GameAttribute.Power_Buff_29_Visual_Effect_E);
					case 30:
						return RuneSelect(GameAttribute.Power_Buff_30_Visual_Effect_None,
										  GameAttribute.Power_Buff_30_Visual_Effect_A,
										  GameAttribute.Power_Buff_30_Visual_Effect_B,
										  GameAttribute.Power_Buff_30_Visual_Effect_C,
										  GameAttribute.Power_Buff_30_Visual_Effect_D,
										  GameAttribute.Power_Buff_30_Visual_Effect_E);
					case 31:
						return RuneSelect(GameAttribute.Power_Buff_31_Visual_Effect_None,
										  GameAttribute.Power_Buff_31_Visual_Effect_A,
										  GameAttribute.Power_Buff_31_Visual_Effect_B,
										  GameAttribute.Power_Buff_31_Visual_Effect_C,
										  GameAttribute.Power_Buff_31_Visual_Effect_D,
										  GameAttribute.Power_Buff_31_Visual_Effect_E);
				}
			}
		}

		private GameAttributeI _Buff_Icon_Start_TickN
		{
			get
			{
				switch (BuffSlot)
				{
					default:
					case 0: return GameAttribute.Buff_Icon_Start_Tick0;
					case 1: return GameAttribute.Buff_Icon_Start_Tick1;
					case 2: return GameAttribute.Buff_Icon_Start_Tick2;
					case 3: return GameAttribute.Buff_Icon_Start_Tick3;
					case 4: return GameAttribute.Buff_Icon_Start_Tick4;
					case 5: return GameAttribute.Buff_Icon_Start_Tick5;
					case 6: return GameAttribute.Buff_Icon_Start_Tick6;
					case 7: return GameAttribute.Buff_Icon_Start_Tick7;
					case 8: return GameAttribute.Buff_Icon_Start_Tick8;
					case 9: return GameAttribute.Buff_Icon_Start_Tick9;
					case 10: return GameAttribute.Buff_Icon_Start_Tick10;
					case 11: return GameAttribute.Buff_Icon_Start_Tick11;
					case 12: return GameAttribute.Buff_Icon_Start_Tick12;
					case 13: return GameAttribute.Buff_Icon_Start_Tick13;
					case 14: return GameAttribute.Buff_Icon_Start_Tick14;
					case 15: return GameAttribute.Buff_Icon_Start_Tick15;
					case 16: return GameAttribute.Buff_Icon_Start_Tick16;
					case 17: return GameAttribute.Buff_Icon_Start_Tick17;
					case 18: return GameAttribute.Buff_Icon_Start_Tick18;
					case 19: return GameAttribute.Buff_Icon_Start_Tick19;
					case 20: return GameAttribute.Buff_Icon_Start_Tick20;
					case 21: return GameAttribute.Buff_Icon_Start_Tick21;
					case 22: return GameAttribute.Buff_Icon_Start_Tick22;
					case 23: return GameAttribute.Buff_Icon_Start_Tick23;
					case 24: return GameAttribute.Buff_Icon_Start_Tick24;
					case 25: return GameAttribute.Buff_Icon_Start_Tick25;
					case 26: return GameAttribute.Buff_Icon_Start_Tick26;
					case 27: return GameAttribute.Buff_Icon_Start_Tick27;
					case 28: return GameAttribute.Buff_Icon_Start_Tick28;
					case 29: return GameAttribute.Buff_Icon_Start_Tick29;
					case 30: return GameAttribute.Buff_Icon_Start_Tick30;
					case 31: return GameAttribute.Buff_Icon_Start_Tick31;
				}
			}
		}

		private GameAttributeI _Buff_Icon_End_TickN
		{
			get
			{
				switch (BuffSlot)
				{
					default:
					case 0: return GameAttribute.Buff_Icon_End_Tick0;
					case 1: return GameAttribute.Buff_Icon_End_Tick1;
					case 2: return GameAttribute.Buff_Icon_End_Tick2;
					case 3: return GameAttribute.Buff_Icon_End_Tick3;
					case 4: return GameAttribute.Buff_Icon_End_Tick4;
					case 5: return GameAttribute.Buff_Icon_End_Tick5;
					case 6: return GameAttribute.Buff_Icon_End_Tick6;
					case 7: return GameAttribute.Buff_Icon_End_Tick7;
					case 8: return GameAttribute.Buff_Icon_End_Tick8;
					case 9: return GameAttribute.Buff_Icon_End_Tick9;
					case 10: return GameAttribute.Buff_Icon_End_Tick10;
					case 11: return GameAttribute.Buff_Icon_End_Tick11;
					case 12: return GameAttribute.Buff_Icon_End_Tick12;
					case 13: return GameAttribute.Buff_Icon_End_Tick13;
					case 14: return GameAttribute.Buff_Icon_End_Tick14;
					case 15: return GameAttribute.Buff_Icon_End_Tick15;
					case 16: return GameAttribute.Buff_Icon_End_Tick16;
					case 17: return GameAttribute.Buff_Icon_End_Tick17;
					case 18: return GameAttribute.Buff_Icon_End_Tick18;
					case 19: return GameAttribute.Buff_Icon_End_Tick19;
					case 20: return GameAttribute.Buff_Icon_End_Tick20;
					case 21: return GameAttribute.Buff_Icon_End_Tick21;
					case 22: return GameAttribute.Buff_Icon_End_Tick22;
					case 23: return GameAttribute.Buff_Icon_End_Tick23;
					case 24: return GameAttribute.Buff_Icon_End_Tick24;
					case 25: return GameAttribute.Buff_Icon_End_Tick25;
					case 26: return GameAttribute.Buff_Icon_End_Tick26;
					case 27: return GameAttribute.Buff_Icon_End_Tick27;
					case 28: return GameAttribute.Buff_Icon_End_Tick28;
					case 29: return GameAttribute.Buff_Icon_End_Tick29;
					case 30: return GameAttribute.Buff_Icon_End_Tick30;
					case 31: return GameAttribute.Buff_Icon_End_Tick31;
				}
			}
		}

		private GameAttributeI _Buff_Icon_CountN
		{
			get
			{
				switch (BuffSlot)
				{
					default:
					case 0: return GameAttribute.Buff_Icon_Count0;
					case 1: return GameAttribute.Buff_Icon_Count1;
					case 2: return GameAttribute.Buff_Icon_Count2;
					case 3: return GameAttribute.Buff_Icon_Count3;
					case 4: return GameAttribute.Buff_Icon_Count4;
					case 5: return GameAttribute.Buff_Icon_Count5;
					case 6: return GameAttribute.Buff_Icon_Count6;
					case 7: return GameAttribute.Buff_Icon_Count7;
					case 8: return GameAttribute.Buff_Icon_Count8;
					case 9: return GameAttribute.Buff_Icon_Count9;
					case 10: return GameAttribute.Buff_Icon_Count10;
					case 11: return GameAttribute.Buff_Icon_Count11;
					case 12: return GameAttribute.Buff_Icon_Count12;
					case 13: return GameAttribute.Buff_Icon_Count13;
					case 14: return GameAttribute.Buff_Icon_Count14;
					case 15: return GameAttribute.Buff_Icon_Count15;
					case 16: return GameAttribute.Buff_Icon_Count16;
					case 17: return GameAttribute.Buff_Icon_Count17;
					case 18: return GameAttribute.Buff_Icon_Count18;
					case 19: return GameAttribute.Buff_Icon_Count19;
					case 20: return GameAttribute.Buff_Icon_Count20;
					case 21: return GameAttribute.Buff_Icon_Count21;
					case 22: return GameAttribute.Buff_Icon_Count22;
					case 23: return GameAttribute.Buff_Icon_Count23;
					case 24: return GameAttribute.Buff_Icon_Count24;
					case 25: return GameAttribute.Buff_Icon_Count25;
					case 26: return GameAttribute.Buff_Icon_Count26;
					case 27: return GameAttribute.Buff_Icon_Count27;
					case 28: return GameAttribute.Buff_Icon_Count28;
					case 29: return GameAttribute.Buff_Icon_Count29;
					case 30: return GameAttribute.Buff_Icon_Count30;
					case 31: return GameAttribute.Buff_Icon_Count31;
				}
			}
		}
	}
}

using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(230745)]
	[ImplementsPowerBuff(0, true)]
	public class NephalemValorBuff : PowerBuff
	{
		public override void Init()
		{
			base.Init();
			MaxStackCount = 3;
			IsPersistent = true;
			Timeout = WaitSeconds(120f);
		}

		private float _currentBonus = 0f;

		public override bool Apply()
		{
			base.Apply();
			_currentBonus = 0.033f * StackCount;
			Target.Attributes[GameAttributes.Magic_Find] += _currentBonus;
			Target.Attributes[GameAttributes.Gold_Find] += _currentBonus;
			Target.Attributes[GameAttributes.Experience_Bonus_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttributes.Magic_Find] -= _currentBonus;
			Target.Attributes[GameAttributes.Gold_Find] -= _currentBonus;
			Target.Attributes[GameAttributes.Experience_Bonus_Percent] -= _currentBonus;

			_currentBonus = 0.033f * StackCount;
			Target.Attributes[GameAttributes.Magic_Find] += _currentBonus;
			Target.Attributes[GameAttributes.Gold_Find] += _currentBonus;
			Target.Attributes[GameAttributes.Experience_Bonus_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttributes.Magic_Find] -= _currentBonus;
			Target.Attributes[GameAttributes.Gold_Find] -= _currentBonus;
			Target.Attributes[GameAttributes.Experience_Bonus_Percent] -= _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();
		}
	}
}

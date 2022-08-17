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
			_currentBonus = 0.033f * this.StackCount;
			Target.Attributes[GameAttribute.Magic_Find] += _currentBonus;
			Target.Attributes[GameAttribute.Gold_Find] += _currentBonus;
			Target.Attributes[GameAttribute.Experience_Bonus_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();
			return true;
		}

		public override bool Stack(Buff buff)
		{
			bool stacked = StackCount < MaxStackCount;

			base.Stack(buff);

			if (!stacked) return true;

			Target.Attributes[GameAttribute.Magic_Find] -= _currentBonus;
			Target.Attributes[GameAttribute.Gold_Find] -= _currentBonus;
			Target.Attributes[GameAttribute.Experience_Bonus_Percent] -= _currentBonus;

			_currentBonus = 0.033f * this.StackCount;
			Target.Attributes[GameAttribute.Magic_Find] += _currentBonus;
			Target.Attributes[GameAttribute.Gold_Find] += _currentBonus;
			Target.Attributes[GameAttribute.Experience_Bonus_Percent] += _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();

			return true;
		}

		public override void Remove()
		{
			base.Remove();
			Target.Attributes[GameAttribute.Magic_Find] -= _currentBonus;
			Target.Attributes[GameAttribute.Gold_Find] -= _currentBonus;
			Target.Attributes[GameAttribute.Experience_Bonus_Percent] -= _currentBonus;
			User.Attributes.BroadcastChangedIfRevealed();
		}
	}
}

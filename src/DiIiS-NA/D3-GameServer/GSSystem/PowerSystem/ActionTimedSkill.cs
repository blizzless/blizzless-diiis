using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class ActionTimedSkill : Skill
	{
		public override float GetContactDelay()
		{
			float actionSpeed = GetActionSpeed();
			if (actionSpeed > 0f)
				return 0.5f / actionSpeed;
			else
				return 0f;
		}
	}
}

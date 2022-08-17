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

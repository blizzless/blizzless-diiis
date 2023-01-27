using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.AISystem.Brains
{
	public class FollowerBrain : Brain
	{
		protected FollowerBrain(ActorSystem.Actor body)
			: base(body)
		{ }

		public override void Think(int tickCounter)
		{ }
	}
}

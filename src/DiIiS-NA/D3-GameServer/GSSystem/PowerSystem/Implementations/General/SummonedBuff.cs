//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;
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
	[ImplementsPowerSNO(30540)]
	public class SummonedBuff : TimedBuff
	{
		public override void Init()
		{
			base.Init();
			Timeout = WaitSeconds(3f);  // TODO: calculate this based on spawn animation length
		}

		public override bool Apply()
		{
			base.Apply();

			Target.HasLoot = false;
			// lookup and play spawn animation, otherwise fail
			if (Target.AnimationSet != null && Target.AnimationSet.TagExists(AnimationTags.Spawn))
			{
				Target.PlayActionAnimation(Target.AnimationSet.GetAniSNO(AnimationTags.Spawn));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

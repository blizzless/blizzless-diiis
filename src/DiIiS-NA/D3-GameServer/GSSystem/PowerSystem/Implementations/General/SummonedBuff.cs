//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats;

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

			this.Target.HasLoot = false;
			// lookup and play spawn animation, otherwise fail
			if (this.Target.AnimationSet != null && this.Target.AnimationSet.TagExists(AnimationTags.Spawn))
			{
				this.Target.PlayActionAnimation(this.Target.AnimationSet.GetAniSNO(AnimationTags.Spawn));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}

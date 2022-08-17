//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
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
	public abstract class PowerScript : PowerContext
	{
		public Vector3D TargetPosition;
		public TargetMessage TargetMessage;
		// Called to start executing a power
		// Yields timers that signify when to continue execution.
		public abstract IEnumerable<TickTimer> Run();

		// token instance that can be yielded by Run() to indicate the power manager should stop
		// running a power implementation.
		public static readonly TickTimer StopExecution = null;


		public TargetList GetBestMeleeEnemy(float meleeRangeBasic = 10f)
		{

			float meleeRange = meleeRangeBasic + User.ActorData.Cylinder.Ax2;  // TODO: possibly use equipped weapon range for this?

			// get all targets that could be hit by melee attack, then select the script's target if
			// it has one, otherwise use the closest target in range.
			TargetList targets = GetEnemiesInBeamDirection(User.Position, TargetPosition, meleeRange);

			Actor bestEnemy;
			if (targets.Actors.Contains(Target))
				bestEnemy = Target;
			else
				bestEnemy = targets.GetClosestTo(User.Position);

			targets.Actors.RemoveAll(actor => actor != bestEnemy);
			return targets;
		}
	}
}

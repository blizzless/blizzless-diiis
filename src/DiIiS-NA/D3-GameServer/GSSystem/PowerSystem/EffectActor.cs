//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
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
	public class EffectActor : Actor, IUpdateable
	{
		public PowerContext Context;

		public TickTimer Timeout = null;
		public float UpdateDelay = 0f;
		public Action OnUpdate = null;
		public Action OnTimeout = null;
		public int UtilityValue = 0;
		public List<Actor> TriggeredActors = new List<Actor>();

		public override ActorType ActorType { get { return ActorType.ClientEffect; } }

		private TickTimer _updateTimer;

		public EffectActor(PowerContext context, ActorSno actorSNO, Vector3D position)
			: base(context.World, actorSNO)
		{
			Context = context;

			Field2 = 0x8;
			if (Scale == 0f)
				Scale = 1f;
			Position = position;

			// copy in important effect params from user
			if (context != null)
				if (context.PowerSNO != 0)
				{
					Attributes[GameAttribute.Rune_A, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_A, context.PowerSNO];
					Attributes[GameAttribute.Rune_B, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_B, context.PowerSNO];
					Attributes[GameAttribute.Rune_C, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_C, context.PowerSNO];
					Attributes[GameAttribute.Rune_D, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_D, context.PowerSNO];
					Attributes[GameAttribute.Rune_E, context.PowerSNO] = context.User.Attributes[GameAttribute.Rune_E, context.PowerSNO];
				}
		}

		public void Spawn(float facingAngle = 0)
		{
			SetFacingRotation(facingAngle);
			World.Enter(this);
		}

		public virtual void Update(int tickCounter)
		{
			if (Timeout != null && Timeout.TimedOut)
			{
				if (OnTimeout != null)
					OnTimeout();

				Destroy();
			}
			else if (OnUpdate != null)
			{
				if (_updateTimer == null || _updateTimer.TimedOut)
				{
					OnUpdate();
					if (UpdateDelay > 0f)
						_updateTimer = new SecondsTickTimer(Context.World.Game, UpdateDelay);
					else
						_updateTimer = null;
				}
			}
		}
	}
}

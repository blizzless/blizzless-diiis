

//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public abstract class Payload
	{
		public PowerContext Context;
		public Actor Target;

		public Payload(PowerContext context, Actor target)
		{
			this.Context = context;
			this.Target = target;
		}
	}
}

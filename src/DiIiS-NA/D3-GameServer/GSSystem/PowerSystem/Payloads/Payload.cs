

using DiIiS_NA.GameServer.GSSystem.ActorSystem;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Payloads
{
	public abstract class Payload
	{
		public PowerContext Context;
		public Actor Target;

		public Payload(PowerContext context, Actor target)
		{
			Context = context;
			Target = target;
		}
	}
}

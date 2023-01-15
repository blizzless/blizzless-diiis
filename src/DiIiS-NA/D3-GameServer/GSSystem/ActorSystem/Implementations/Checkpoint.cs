//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Checkpoint : Gizmo
	{
		private bool _checkpointReached = false;

		public Checkpoint(World world, int snoId, TagMap tags)
			: base(world, snoId, tags, false)
		{

		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			if (this.World.Game.PvP) return;
			if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * this.Scale * this.Scale && !_checkpointReached)
			{
				_checkpointReached = true;

				player.InGameClient.SendMessage(new PlayEffectMessage
				{
					ActorId = player.DynamicID(player),
					Effect = Effect.Checkpoint
				});

				player.CheckPointPosition = this.Position;
				player.Attributes[GameAttribute.Corpse_Resurrection_Charges] = 3;     // Reset corpse resurrection charges
			}
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			return false;
		}
	}
}

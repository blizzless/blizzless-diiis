using System.Drawing;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc;
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	class Savepoint : Gizmo
	{
		private bool _savepointReached = false;

		public int SavepointId { get; private set; }

		public int SNOLevelArea = -1;

		public Savepoint(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags, false)
		{
			SavepointId = tags[MarkerKeys.SavepointId];
			var proximity = new RectangleF(Position.X - 1f, Position.Y - 1f, 2f, 2f);
			var scenes = World.QuadTree.Query<Scene>(proximity);
			if (scenes.Count == 0) return; // TODO: fixme! /raist
			var scene = scenes[0]; // Parent scene /fasbat
			SNOLevelArea = scene.Specification.SNOLevelAreas[0];
		}

		public override void OnPlayerApproaching(PlayerSystem.Player player)
		{
			if (player.Position.DistanceSquared(ref _position) < ActorData.Sphere.Radius * ActorData.Sphere.Radius * Scale * Scale && !_savepointReached)
			{
				_savepointReached = true;

				// TODO send real SavePointInformation, though im not sure if that is used for anything at all

				player.InGameClient.SendMessage(new SavePointInfoMessage
				{
					snoLevelArea = SNOLevelArea,//102362,
				});

				player.SavePointData = new SavePointData() { snoWorld = (int)World.SNO, SavepointId = SavepointId };
				player.UpdateHeroState();
				player.CheckPointPosition = _position; // This seemed easier than having on Death find the SavePoint based on ID, then getting its location. - DarkLotus
			}
		}

		public override bool Reveal(PlayerSystem.Player player)
		{
			return false;
		}
	}
}

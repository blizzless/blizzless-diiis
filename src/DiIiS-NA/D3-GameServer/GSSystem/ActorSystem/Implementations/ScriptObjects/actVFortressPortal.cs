//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PowerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Drawing;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._x1_fortress_portal_switch)]
	public class ActVFortressPortal : Gizmo
	{
		public ActVFortressPortal(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.Attributes[GameAttribute.MinimapActive] = true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);

			var proximity = new RectangleF(this.Position.X - 1f, this.Position.Y - 1f, 2f, 2f);
			var scene = this.World.QuadTree.Query<Scene>(proximity).First();

			var portals = Scene.PreCachedMarkers[scene.SceneSNO.Id].Where(m => m.SNOHandle.Id == 328830).Select(m => m.PRTransform.Vector3D).ToList();
			var destinations = Scene.PreCachedMarkers[scene.SceneSNO.Id].Where(m => m.Name.Contains("_Destination")).Select(m => m.PRTransform.Vector3D).ToList();

			int i = 0;
			int n = 0;

			float closestDistance = float.MaxValue;
			foreach (var portal_pos in portals)
			{
				float distance = PowerMath.Distance2D((portal_pos + scene.Position), this.Position);
				if (distance < closestDistance)
				{
					n = i;
					closestDistance = distance;
				}
				i++;
			}

			var destination_position = destinations[n];

			player.Teleport(destination_position + scene.Position);
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;
			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName(string.Format("{0}-{1}", this.Name, this.GlobalID)),
				Place = new WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
				ImageInfo = 377766,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = (int)this.SNO,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = false,
				PlayerUseFlags = 0
			});
			/*
			player.InGameClient.SendMessage(new MapMarkerInfoMessage
			{
				HashedName = StringHashHelper.HashItemName(string.Format("{0}-{1}", this.ActorSNO.Name, this.GlobalID)),
				Place = new WorldPlace { Position = this.Position, WorldID = this.World.GlobalID },
				ImageInfo = 339889,
				Label = -1,
				snoStringList = -1,
				snoKnownActorOverride = this.ActorSNO.Id,
				snoQuestSource = -1,
				Image = -1,
				Active = true,
				CanBecomeArrow = false,
				RespectsFoW = false,
				IsPing = false,
				PlayerUseFlags = 0
			});
			//*/
			return true;
		}
	}
}

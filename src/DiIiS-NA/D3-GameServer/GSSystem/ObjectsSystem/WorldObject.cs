using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
	public abstract class WorldObject : DynamicObject, IRevealable, IDisposable
	{
		/// <summary>
		/// The world object belongs to.
		/// </summary>
		public World World { get; set; }

		protected Vector3D _position;

		/// <summary>
		/// The position of the object.
		/// </summary>
		public Vector3D Position
		{
			get => _position;
			set
			{
				_position = value;
				if (_position == null) return;
				Bounds = new RectangleF(Position.X, Position.Y, Size.Width, Size.Height);
				var handler = PositionChanged;
				if (handler != null) handler(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Event handler for position-change.
		/// </summary>
		public event EventHandler PositionChanged;

		/// <summary>
		/// Size of the object.
		/// </summary>
		public Size Size { get; protected set; }

		/// <summary>
		/// Automatically calculated bounds for object used by QuadTree.
		/// </summary>
		public RectangleF Bounds { get; private set; }

		/// <summary>
		/// Scale of the object.
		/// </summary>
		public float Scale { get; set; }

		public Vector3D RotationAxis { get; set; }

		public float RotationW { get; set; }

		/// <summary>
		/// Creates a new world object.
		/// </summary>
		/// <param name="world">The world object belongs to.</param>
		/// <param name="dynamicID">The dynamicId of the object.</param>
		protected WorldObject(World world, uint dynamicID)
			: base(dynamicID)
		{
			if (world == null) return;
			//if (dynamicID == 0)
			//this.DynamicID = world.NewActorID;
			World = world;
			//this.World.StartTracking(this); // track the object.
			RotationAxis = new Vector3D(); 
			_position = new Vector3D();
		}

		/// <summary>
		/// Reveals the object to given player.
		/// </summary>
		/// <param name="player">The player to reveal the object.</param>
		/// <returns>true if the object was revealed or false if the object was already revealed.</returns>
		public abstract bool Reveal(Player player);

		/// <summary>
		/// Unreveals the object to given plaer.
		/// </summary>
		/// <param name="player">The player to unreveal the object.</param>
		/// <returns>true if the object was unrevealed or false if the object wasn't already revealed.</returns>
		public abstract bool Unreveal(Player player);

		/// <summary>
		/// Makes the object leave the world and then destroys it.
		/// </summary>
		public override void Destroy()
		{
			try
			{
				if (this is Actor actor)
					if (World != null)
						World.Leave(actor);

				//this.World.EndTracking(this);
			}
			catch { }
			World = null;
		}


		public void Dispose()
		{
			Destroy();
		}
	}
}

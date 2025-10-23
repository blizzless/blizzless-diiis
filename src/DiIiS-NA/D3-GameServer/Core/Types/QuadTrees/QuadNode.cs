using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;

namespace DiIiS_NA.GameServer.Core.Types.QuadTrees
{
	public class QuadNode
	{
		/// <summary>
		/// Parent node.
		/// </summary>
		public QuadNode Parent { get; internal set; }

		/// <summary>
		/// Childs nodes.
		/// </summary>
		private readonly QuadNode[] _nodes = new QuadNode[4];

		/// <summary>
		/// Read only collection of nodes.
		/// </summary>
		public ReadOnlyCollection<QuadNode> Nodes;

		/// <summary>
		/// Child node in given direction.
		/// </summary>
		/// <param name="direction"><see cref="Direction"/></param>
		/// <returns>The child node.</returns>
		public QuadNode this[Direction direction]
		{
			get
			{
				switch (direction)
				{
					case Direction.NorthWest:
						return _nodes[0];
					case Direction.NorthEast:
						return _nodes[1];
					case Direction.SouthWest:
						return _nodes[2];
					case Direction.SouthEast:
						return _nodes[3];
					default:
						return null;
				}
			}
			set
			{
				switch (direction)
				{
					case Direction.NorthWest:
						_nodes[0] = value;
						break;
					case Direction.NorthEast:
						_nodes[1] = value;
						break;
					case Direction.SouthWest:
						_nodes[2] = value;
						break;
					case Direction.SouthEast:
						_nodes[3] = value;
						break;
				}
				if (value != null)
					value.Parent = this;
			}
		}

		/// <summary>
		/// List of contained objects.
		/// </summary>
		public ConcurrentDictionary<uint, WorldObject> ContainedObjects = new ConcurrentDictionary<uint, WorldObject>();

		/// <summary>
		/// The bounds for node.
		/// </summary>
		public RectangleF Bounds { get; internal set; }

		/// <summary>
		/// Creates a new QuadNode with given bounds.
		/// </summary>
		/// <param name="bounds">The bounds for node.</param>
		public QuadNode(RectangleF bounds)
		{
			Bounds = bounds;
			Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
		}

		/// <summary>
		/// Creates a new QuadNode with given bounds parameters.
		/// </summary>
		/// <param name="x">The x-coordinate of top-left corner of the region.</param>
		/// <param name="y">The y-coordinate of top-left corner of the region</param>
		/// <param name="width">The width of the region.</param>
		/// <param name="height">The height of the region</param>
		public QuadNode(double x, double y, double width, double height)
			: this(new RectangleF((float)x, (float)y, (float)width, (float)height))
		{ }

		/// <summary>
		/// Returns true if node has child-nodes.
		/// </summary>
		/// <returns><see cref="bool"/></returns>
		public bool HasChildNodes()
		{
			return _nodes[0] != null;
		}
	}
}

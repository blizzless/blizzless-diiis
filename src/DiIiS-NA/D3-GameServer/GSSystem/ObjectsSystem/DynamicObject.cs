using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ObjectsSystem
{
	public abstract class DynamicObject
	{
		/// <summary>
		/// The global unique runtime ID for the actor.
		/// </summary>
		public uint GlobalID
		{
			get
			{
				if (GlobalIDOverride > 0)
					return GlobalIDOverride;
				else
					return _globalID;
			}
			private set
			{ }
		}

		private uint _globalID;
		public uint GlobalIDOverride;

		/// <summary>
		/// The dynamic non-unique runtime ID for the actor.
		/// </summary>
		public uint DynamicID(Player plr)
		{
			if (this is Player && (!(this as Player).IsInPvPWorld || this == plr))
				return (uint)(this as Player).PlayerIndex;
			//if(plr.RevealedObjects.ContainsKey(this.))
			return plr.RevealedObjects[GlobalID];
		}

		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="dynamicID">The dynamic ID to initialize with.</param>
		protected DynamicObject(uint globalID)
		{
			_globalID = globalID;
			GlobalIDOverride = 0;
		}

		/// <summary>
		/// Destroy the object. This should remove any references to the object throughout GS.
		/// </summary>
		public abstract void Destroy();
	}
}

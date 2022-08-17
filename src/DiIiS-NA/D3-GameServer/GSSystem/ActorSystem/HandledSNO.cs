//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class HandledSNOAttribute : Attribute
	{
		public List<int> SNOIds { get; private set; }

		public HandledSNOAttribute(params int[] snoIds)
		{
			this.SNOIds = new List<int>();
			this.SNOIds.AddRange(snoIds);
		}
	}
}

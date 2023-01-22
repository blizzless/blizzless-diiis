//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class HandledSNOAttribute : Attribute
	{
		public List<ActorSno> SNOIds { get; private set; }

		public HandledSNOAttribute(params ActorSno[] snoIds)
		{
			SNOIds = new List<ActorSno>();
			SNOIds.AddRange(snoIds);
		}
	}
}

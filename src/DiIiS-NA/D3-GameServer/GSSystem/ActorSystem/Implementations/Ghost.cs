//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
	129345, //maghda
	5360, //Leoric ghost
	211014, //Maghda event
	215247, //Diablo_EndGame
	186130, //DemonVoiceover
	321479, 321451, 321454, //A5 voice actors
	175310, //PT_Mystic_NoVendor_NonGlobalFollower
	340101, //x1_Urzael_Invisible
	373456 //Malthael ghost
	)]
	public class Ghost : InteractiveNPC
	{
		public Ghost(MapSystem.World world, int snoId, TagMap tags)
			: base(world, snoId, tags)
		{
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
		}
	}
}

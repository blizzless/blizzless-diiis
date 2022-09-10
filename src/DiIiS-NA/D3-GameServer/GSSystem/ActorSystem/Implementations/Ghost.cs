//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(
		ActorSno._maghda_a_tempprojection, //maghda
		ActorSno._skeletonking_ghost, //Leoric ghost
		ActorSno._maghda_nolaugh, //Maghda event
		ActorSno._diablo_endgame, //Diablo_EndGame
		ActorSno._demonvoiceover, //DemonVoiceover
		ActorSno._x1_westm_heroworship03_vo, ActorSno._x1_westm_heroworship01_vo, ActorSno._x1_westm_heroworship02_vo, //A5 voice actors
		ActorSno._pt_mystic_novendor_nonglobalfollower, //PT_Mystic_NoVendor_NonGlobalFollower
		ActorSno._x1_urzael_invisible, //x1_Urzael_Invisible
		ActorSno._x1_malthael_deathorbevent //Malthael ghost
	)]
	public class Ghost : InteractiveNPC
	{
		public Ghost(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.CollFlags = 0;
			this.WalkSpeed = 0;
			this.Attributes[GameAttribute.Invulnerable] = true;
		}
	}
}

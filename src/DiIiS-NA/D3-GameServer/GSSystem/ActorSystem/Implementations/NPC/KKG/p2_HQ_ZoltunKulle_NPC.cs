using System;
using System.Linq;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._p2_hq_zoltunkulle_npc)] //p2_HQ_ZoltunKulle_NPC
	public class P2_HQ_ZoltunKulle_NPC : NPC
	{

		public P2_HQ_ZoltunKulle_NPC(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Hidden = true;
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
	}
}

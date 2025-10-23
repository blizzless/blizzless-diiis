using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.MessageSystem;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	[HandledSNO(ActorSno._cain)] //Cain
	public class Cain : InteractiveNPC
	{
		public Cain(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttributes.Invulnerable] = true;
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
	}
}

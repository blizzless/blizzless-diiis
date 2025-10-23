using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
    //[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 449323 Name: Barbarian_KKG_Event, NumInWorld: 0
    //[ Info] [AttackPayload]: Игрок с индесом: 0 - задамажил: ID: 435818 Name: Barbarian_KKG, NumInWorld: 0
    [HandledSNO(ActorSno._barbarian_kkg, ActorSno._barbarian_kkg_event)] //Barbarian_KKG
	public class Barbarian_KKG : NPC
	{
		public Barbarian_KKG(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			this.PlayActionAnimation(AnimationSno.barbarian_kkg_follower_hth_kkgevent_sit);
		}

		protected override void ReadTags()
		{
			if (!Tags.ContainsKey(MarkerKeys.ConversationList))
				Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));

			base.ReadTags();
		}
	}
}

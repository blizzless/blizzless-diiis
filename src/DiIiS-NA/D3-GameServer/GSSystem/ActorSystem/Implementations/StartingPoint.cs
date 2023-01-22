//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations
{
	public class StartingPoint : Gizmo
	{
		public int TargetId { get; private set; }

		public StartingPoint(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags, false)
		{
		}

		protected override void ReadTags()
		{
			if (Tags == null) return;

			if (Tags.ContainsKey(MarkerKeys.ActorTag))
				TargetId = Tags[MarkerKeys.ActorTag];
		}

		public override bool Reveal(Player player)
		{
			return false;
		}
	}
}

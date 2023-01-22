//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._skeletonkinggizmo)]
	public class Leoric : Gizmo
	{
		public Leoric(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Attributes[GameAttribute.MinimapActive] = true;
		}


		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

			return true;
		}

		public override bool Unreveal(Player player)
		{
			if (!base.Unreveal(player))
				return false;

			return true;
		}

		public override void OnTargeted(Player player, TargetMessage message)
		{
			base.OnTargeted(player, message);
			this.PlayAnimation(5, AnimationSno.skeletonking_spawn_from_throne, 1f);

			bool status = false;

			Attributes[GameAttribute.Team_Override] = (status ? -1 : 2);
			Attributes[GameAttribute.Untargetable] = !status;
			Attributes[GameAttribute.NPC_Is_Operatable] = status;
			Attributes[GameAttribute.Operatable] = status;
			Attributes[GameAttribute.Operatable_Story_Gizmo] = status;
			Attributes[GameAttribute.Disabled] = !status;
			Attributes[GameAttribute.Immunity] = !status;
			Attributes.BroadcastChangedIfRevealed();

			Attributes.BroadcastChangedIfRevealed();
			var ListenerKingSkeletons = Task.Delay(16000).ContinueWith(delegate {
				World.SpawnMonster(ActorSno._skeletonking, Position);
				Destroy();
			});
		}
	}
}

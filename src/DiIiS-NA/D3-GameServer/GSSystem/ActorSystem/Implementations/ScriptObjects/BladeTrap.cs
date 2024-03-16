using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.MessageSystem;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._a1dun_leor_hallway_blade_trap)]
	public class BladeTrap : Monster
	{
		public BladeTrap(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x8;
			CollFlags = 0;
			WalkSpeed = 0;
			Attributes[GameAttributes.Invulnerable] = true;
			//Logger.Debug("Jondar, tagSNO: {0}", tags[MarkerKeys.OnActorSpawnedScript].Id);
		}

	}
}

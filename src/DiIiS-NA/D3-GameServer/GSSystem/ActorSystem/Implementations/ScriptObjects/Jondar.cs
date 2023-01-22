//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.MapSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.ScriptObjects
{
	[HandledSNO(ActorSno._adventurer_d_templarintrounique, ActorSno._khamsin_mine_unique)]
	public class Jondar : Monster
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public Jondar(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			Field2 = 0x8;
			Attributes[GameAttribute.MinimapActive] = true;
			Attributes[GameAttribute.Untargetable] = false;
			Attributes[GameAttribute.Operatable] = true;
			Attributes[GameAttribute.Disabled] = false;
			Attributes[GameAttribute.TeamID] = 10;
			WalkSpeed = 0.1f;
			//Logger.Debug("Jondar, tagSNO: {0}", tags[MarkerKeys.OnActorSpawnedScript].Id);
		}

		public override int Quality
		{
			get
			{
				return (int)DiIiS_NA.Core.MPQ.FileFormats.SpawnType.Unique;
			}
			set
			{
			}
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

	}
}

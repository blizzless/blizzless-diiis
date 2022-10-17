//Blizzless Project 2022 
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations
{
	class SpawnShadows : QuestEvent
	{

		public SpawnShadows()
			: base(0)
		{
		}

		public override void Execute(MapSystem.World world)
		{
			//Logger.Debug("SpawnShadows event started");
			if (world.Game.Empty) return;

			foreach (var plr in world.Players.Values)
			{
				plr.InGameClient.SendMessage(new BoolDataMessage(Opcodes.CameraTriggerFadeToBlackMessage) { Field0 = true });
				plr.InGameClient.SendMessage(new SimpleMessage(Opcodes.CameraSriptedSequenceStopMessage) { });
			}

			var stone = world.GetActorBySNO(ActorSno._a2dun_zolt_black_soulstone);
			for (int i = 0; i < 8; i++)
			{
				float angle = (float)(FastRandom.Instance.NextDouble() * Math.PI * 2);
				float radius = 10f + (float)FastRandom.Instance.NextDouble() * (25f - 10f);
				Vector3D rand_direction = new Vector3D(stone.Position.X + (float)Math.Cos(angle) * radius, stone.Position.Y + (float)Math.Sin(angle) * radius, stone.Position.Z);
				world.SpawnMonster(ActorSno._shadowvermin_soulstoneevent, rand_direction);
			}
		}

	}
}

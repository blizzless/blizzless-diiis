using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using System;

namespace DiIiS_NA.GameServer.GSSystem.QuestSystem.QuestEvents.Implementations.Act_I
{
    class AttackTownBoominHome : QuestEvent
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public AttackTownBoominHome()
			: base(0)
		{

		}

		public override void Execute(MapSystem.World world)
		{
			var leah = world.GetActorBySNO(ActorSno._leahritual);
			
			leah.Attributes[GameAttributes.Damage_Weapon_Min, 0] = 5f;
			leah.Attributes[GameAttributes.Damage_Weapon_Delta, 0] = 5f;
			world.PowerManager.RunPower(leah, 190230);
			//130848
			leah.PlayEffectGroup(130848);
			var summoners = world.GetActorsBySno(ActorSno._triunesummoner_a_cainevent);

            foreach (var cultist in summoners)
            {
                cultist.Attributes[GameAttributes.Hitpoints_Max] *= (float)0.1; // 10% HP
                cultist.Attributes[GameAttributes.Hitpoints_Cur] =
                    cultist.Attributes[GameAttributes.Hitpoints_Max];
            }

			StartConversation(world, 165428);
			//165428
		}

		private bool StartConversation(MapSystem.World world, Int32 conversationId)
		{
			foreach (var player in world.Players)
			{
				player.Value.Conversations.StartConversation(conversationId);
			}
			return true;
		}
	}
}

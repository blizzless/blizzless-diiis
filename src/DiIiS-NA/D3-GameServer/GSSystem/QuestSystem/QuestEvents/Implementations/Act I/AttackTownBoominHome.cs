using DiIiS_NA.Core.Logging;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			var Leah = world.GetActorBySNO(ActorSno._leahritual);
			
			Leah.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
			Leah.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
			world.PowerManager.RunPower(Leah, 190230);
			//130848
			Leah.PlayEffectGroup(130848);
			var Summoners = world.GetActorsBySNO(ActorSno._triunesummoner_a_cainevent);


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

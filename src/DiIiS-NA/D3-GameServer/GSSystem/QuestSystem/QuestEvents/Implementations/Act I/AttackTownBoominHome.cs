//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
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
			var encWorld = world.Game.GetWorld(194933);

			var Leah = world.GetActorBySNO(121208);
			
			Leah.Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
			Leah.Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
			world.PowerManager.RunPower(Leah, 190230);
			//130848
			Leah.PlayEffectGroup(130848);
			var Summoners = world.GetActorsBySNO(186039);


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

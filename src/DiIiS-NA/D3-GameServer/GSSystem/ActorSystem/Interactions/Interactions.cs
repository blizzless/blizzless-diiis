//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions
{
	public interface IInteraction
	{
		NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player);
	}

	public class ConversationInteraction : IInteraction
	{
		private static readonly Logger Logger = new Logger("ConversationInteraction");

		public int ConversationSNO;
		public bool Read;

		public ConversationInteraction(int conversationSNO, bool forceactive = true)
		{
			ConversationSNO = conversationSNO;
			Read = false; // Should read from players saved data /fasbat
			var Data = (DiIiS_NA.Core.MPQ.FileFormats.Conversation)DiIiS_NA.Core.MPQ.MPQStorage.Data.Assets[Core.Types.SNO.SNOGroup.Conversation][conversationSNO].Data;

			if (Data.ConversationType == DiIiS_NA.Core.MPQ.FileFormats.ConversationTypes.GlobalFloat && forceactive)
				Read = true;
		}

		public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
		{
			return new NPCInteraction()
			{
				Type = NPCInteractionType.Conversation,
				ConversationSNO = ConversationSNO,
				Field2 = -1,
				State = (Read ? NPCInteractionState.Used : NPCInteractionState.New),
			};
		}

		public void MarkAsRead() // Just a hack to show functionality /fasbat
		{
			//Logger.Debug(" (Mark as Read) ConversationSNO {0} is marked as Read ", ConversationSNO);
			Read = true;
		}
	}

	public class HireInteraction : IInteraction
	{
		public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
		{
			return new NPCInteraction()
			{
				Type = NPCInteractionType.Hire,
				ConversationSNO = -1,
				Field2 = -1,
				State = (npc as Hireling).HasHireling ? NPCInteractionState.New : NPCInteractionState.Disabled
			};
		}
	}

	public class IdentifyAllInteraction : IInteraction
	{
		public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
		{
			return new NPCInteraction()
			{
				Type = NPCInteractionType.IdentifyAll,
				ConversationSNO = -1,
				Field2 = -1,
				State = NPCInteractionState.New // Has items to identify? If no disable,
			};
		}
	}

	public class CraftInteraction : IInteraction
	{
		public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
		{
			return new NPCInteraction()
			{
				Type = NPCInteractionType.Craft,
				ConversationSNO = -1,
				Field2 = -1,
				State = NPCInteractionState.New,
			};
		}
	}

	public class InventoryInteraction : IInteraction
	{
		public NPCInteraction AsNPCInteraction(InteractiveNPC npc, Player player)
		{
			return new NPCInteraction()
			{
				Type = NPCInteractionType.Inventory,
				ConversationSNO = -1,
				Field2 = -1,
				State = (npc as Hireling).HasProxy ? NPCInteractionState.New : NPCInteractionState.Disabled
			};
		}
	}
}

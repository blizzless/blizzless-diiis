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
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerSavedConversationsUpdateMessage)]
    public class PlayerSavedConversationsUpdateMessage : GameMessage
    {
        public SavedConversations Field0;
       
        public override void Parse(GameBitBuffer buffer)
        {
            Field0.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Field0.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

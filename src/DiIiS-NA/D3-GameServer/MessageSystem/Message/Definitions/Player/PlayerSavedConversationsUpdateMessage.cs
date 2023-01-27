using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

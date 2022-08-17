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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
     [Message(new[] { Opcodes.PlayerSetCameraDefaultsMessage })]
    public class PlayerSetCameraDefaultsMessage : GameMessage
    {

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSetCameraDefaultsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

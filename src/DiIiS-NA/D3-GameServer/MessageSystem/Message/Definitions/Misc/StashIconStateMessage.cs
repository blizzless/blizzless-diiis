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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.ServerStashIconStateMessage)]
    public class StashIconStateMessage : GameMessage
    {
        public byte[] StashIcons = new byte[13];

        public StashIconStateMessage() : base(Opcodes.ServerStashIconStateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 13; i++)
                StashIcons[i] = (byte)buffer.ReadInt(8);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 13; i++)
                buffer.WriteInt(8, StashIcons[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("StashIconStateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    [Message(Opcodes.ClientStashIconStateMessage, Consumers.Player)]
    public class StashIconStateAssignMessage : GameMessage
    {
        public byte[] StashIcons = new byte[4];

        public StashIconStateAssignMessage() : base(Opcodes.ClientStashIconStateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 4; i++)
                StashIcons[i] = (byte)buffer.ReadInt(8);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            for (UInt16 i = 0; i < 4; i++)
                buffer.WriteInt(8, StashIcons[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("StashIconStateAssignMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDCollFlagsMessage)]
    public class ACDCollFlagsMessage : GameMessage
    {
        public uint ActorID; // The actor's DynamicID
        public int CollFlags;

        public ACDCollFlagsMessage() : base(Opcodes.ACDCollFlagsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            CollFlags = buffer.ReadInt(13);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(13, CollFlags);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDCollFlagsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("CollFlags: 0x" + CollFlags.ToString("X8") + " (" + CollFlags + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

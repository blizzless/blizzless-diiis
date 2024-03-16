using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDGroupMessage)]
    public class ACDGroupMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID
        public int Group1Hash; //Primary
        public int Group2Hash; //Secondary

        public ACDGroupMessage() : base(Opcodes.ACDGroupMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Group1Hash = buffer.ReadInt(32);
            Group2Hash = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(32, Group1Hash);
            buffer.WriteInt(32, Group2Hash);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDGroupMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + Group1Hash.ToString("X8") + " (" + Group1Hash + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Group2Hash.ToString("X8") + " (" + Group2Hash + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

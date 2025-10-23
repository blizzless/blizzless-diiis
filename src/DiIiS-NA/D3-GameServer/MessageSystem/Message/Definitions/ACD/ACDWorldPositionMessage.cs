using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDWorldPositionMessage)]
    public class ACDWorldPositionMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID
        public WorldLocationMessageData WorldLocation;
        public EnterKnownLookOverrides Field2;

        public ACDWorldPositionMessage() : base(Opcodes.ACDWorldPositionMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            WorldLocation = new WorldLocationMessageData();
            WorldLocation.Parse(buffer);
            if (buffer.ReadBool())
            {
                Field2.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            WorldLocation.Encode(buffer);
            buffer.WriteBool(Field2 != null);
            if (Field2 != null)
            {
                Field2.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDWorldPositionMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            WorldLocation.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

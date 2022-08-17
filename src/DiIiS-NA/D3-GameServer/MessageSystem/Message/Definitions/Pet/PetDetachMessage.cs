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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet
{
    [Message(Opcodes.PetDetachMessage)]
    public class PetDetachMessage : GameMessage
    {
        public uint PetId;
        public bool DisplayChatMessage;

        public PetDetachMessage()
            : base(Opcodes.PetDetachMessage)
        {

        }

        public override void Parse(GameBitBuffer buffer)
        {
            PetId = buffer.ReadUInt(32);
            DisplayChatMessage = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, PetId);
            buffer.WriteBool(DisplayChatMessage);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PetDetachMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PetId: 0x" + PetId.ToString("X8") + " (" + PetId + ")");
            b.Append(' ', pad); b.AppendLine("DisplayChatMessage " + (DisplayChatMessage ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

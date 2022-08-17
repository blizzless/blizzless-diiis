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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Conversation
{
    [Message(Opcodes.StopConvLineMessage)]
    public class StopConvLineMessage : GameMessage
    {
        public int PlayLineParamsId;
        public bool Interrupt;

        public StopConvLineMessage() : base(Opcodes.StopConvLineMessage) { }
        public override void Parse(GameBitBuffer buffer)
        {
            PlayLineParamsId = buffer.ReadInt(32);
            Interrupt = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PlayLineParamsId);
            buffer.WriteBool(Interrupt);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("StopConvLineMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayLineParamsId: 0x" + PlayLineParamsId.ToString("X8") + " (" + PlayLineParamsId + ")");
            b.Append(' ', pad); b.AppendLine("Interrupt: " + (Interrupt ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

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
    [Message(Opcodes.UpdateConvAutoAdvanceMessage, Consumers.Conversations)]
    class UpdateConvAutoAdvanceMessage : GameMessage
    {
        /// <summary>
        /// SNO of the conversation ressource
        /// </summary>
        public int SNOConversation;

        /// <summary>
        /// Identifier of the PlayLineParams as used in PlayConvLineMessage to start the conversation
        /// </summary>
        public int PlayLineParamsId;

        /// <summary>
        /// New end of the conversation
        /// </summar
        public int EndTick;

        public override void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
            PlayLineParamsId = buffer.ReadInt(32);
            EndTick = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
            buffer.WriteInt(32, PlayLineParamsId);
            buffer.WriteInt(32, EndTick);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("UpdateConvAutoAdvanceMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOConversation: 0x" + SNOConversation.ToString("X8") + " (" + SNOConversation + ")");
            b.Append(' ', pad); b.AppendLine("PlayLineParamsId: 0x" + PlayLineParamsId.ToString("X8") + " (" + PlayLineParamsId + ")");
            b.Append(' ', pad); b.AppendLine("EndTick: 0x" + EndTick.ToString("X8") + " (" + EndTick + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

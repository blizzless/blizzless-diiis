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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling
{
    [Message(Opcodes.HirelingRequestLearnSkillMessage, Consumers.Player)]
    public class HirelingRequestLearnSkillMessage : GameMessage
    {
        public uint HirelingID; //Actor ID of the hireling
        public int /* sno */ PowerSNOId;

        public HirelingRequestLearnSkillMessage() : base(Opcodes.HirelingRequestLearnSkillMessage)
        {
        }

        public override void Parse(GameBitBuffer buffer)
        {
            HirelingID = buffer.ReadUInt(32);
            PowerSNOId = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, HirelingID);
            buffer.WriteInt(32, PowerSNOId);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingRequestLearnSkillMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HirelingID: 0x" + HirelingID.ToString("X8") + " (" + HirelingID + ")");
            b.Append(' ', pad); b.AppendLine("PowerSNOId: 0x" + PowerSNOId.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

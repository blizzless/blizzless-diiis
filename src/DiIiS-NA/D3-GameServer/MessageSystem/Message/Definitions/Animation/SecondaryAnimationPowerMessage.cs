//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation
{
    [Message(Opcodes.SecondaryAnimationPowerMessage, Consumers.Player)]
    public class SecondaryAnimationPowerMessage : GameMessage
    {
        public int /* sno */ PowerSNO;
        public int annTarget;
        public AnimPreplayData AnimPreplayData;

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNO = buffer.ReadInt(32);
            annTarget = buffer.ReadInt(32);
            if (buffer.ReadBool())
            {
                AnimPreplayData = new AnimPreplayData();
                AnimPreplayData.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteInt(32, annTarget);
            buffer.WriteBool(AnimPreplayData != null);
            if (AnimPreplayData != null)
            {
                AnimPreplayData.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SecondaryAnimationPowerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            if (AnimPreplayData != null)
            {
                AnimPreplayData.AsText(b, pad);
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }

    [Message(Opcodes.ExecuteNonPrimaryPowerMessage, Consumers.Player)]
    public class MiscPowerMessage : GameMessage
    {
        public int /* sno */ PowerSNO;

        public override void Parse(GameBitBuffer buffer)
        {
            PowerSNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PowerSNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("MiscPowerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

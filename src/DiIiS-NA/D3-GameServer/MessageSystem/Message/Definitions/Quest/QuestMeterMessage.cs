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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(Opcodes.QuestMeterMessage)]
    public class QuestMeterMessage : GameMessage
    {
        public int /* sno */ snoQuest;
        public int annMeter;
        public float flMeter;

        public QuestMeterMessage() : base(Opcodes.QuestMeterMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoQuest = buffer.ReadInt(32);
            annMeter = buffer.ReadInt(32);
            flMeter = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoQuest);
            buffer.WriteInt(32, annMeter);
            buffer.WriteFloat32(flMeter);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("QuestMeterMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoQuest: 0x" + snoQuest.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("annMeter: 0x" + annMeter.ToString("X8") + " (" + annMeter + ")");
            b.Append(' ', pad); b.AppendLine("flMeter: " + flMeter.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

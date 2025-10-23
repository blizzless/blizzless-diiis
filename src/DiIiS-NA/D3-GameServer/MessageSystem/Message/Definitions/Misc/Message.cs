    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.CombatLogEntryMessage)]
    class CombatLogEntryMessage : GameMessage
    {
        public int EventType;
        public int ACDSource;
        public int ACDTarget;
        public int snoData;
        public float flParam1;
        public float flParam2;
        public int Dec;

        public override void Parse(GameBitBuffer buffer)
        {
            EventType = buffer.ReadInt(32);
            ACDSource = buffer.ReadInt(32);
            ACDTarget = buffer.ReadInt(32);
            snoData = buffer.ReadInt(32);
            flParam1 = buffer.ReadFloat32();
            flParam2 = buffer.ReadFloat32();
            Dec = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, EventType);
            buffer.WriteInt(32, ACDSource);
            buffer.WriteInt(32, ACDTarget);
            buffer.WriteInt(32, snoData);
            buffer.WriteFloat32(flParam1);
            buffer.WriteFloat32(flParam2);
            buffer.WriteInt(32, Dec);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("CombatLogEntryMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("EventType: 0x" + EventType.ToString("X8") + " (" + EventType + ")");
            b.Append(' ', pad); b.AppendLine("ACDSource: 0x" + ACDSource.ToString("X8") + " (" + ACDSource + ")");
            b.Append(' ', pad); b.AppendLine("ACDTarget: 0x" + ACDTarget.ToString("X8") + " (" + ACDTarget + ")");
            b.Append(' ', pad); b.AppendLine("snoData: 0x" + snoData.ToString("X8") + " (" + snoData + ")");
            b.Append(' ', pad); b.AppendLine("flParam1: " + flParam1.ToString("G"));
            b.Append(' ', pad); b.AppendLine("flParam2: " + flParam2.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Dec: 0x" + Dec.ToString("X8") + " (" + Dec + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

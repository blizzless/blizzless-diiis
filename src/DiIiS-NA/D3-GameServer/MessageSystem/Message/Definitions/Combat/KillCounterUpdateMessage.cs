using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.KillCounterUpdateMessage)]
    public class KillCounterUpdateMessage : GameMessage
    {
        public int BonusType; //Bonus-Type
        /*
        0: Massacre
        1: Destruction
        2: Mighty Blow
        3: Pulverized
        */
        public int KilledCount; //Monsters killed
        public float XPMultiplier;
        public int TotalTime; 
        public bool Expired;

        public KillCounterUpdateMessage() : base(Opcodes.KillCounterUpdateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            BonusType = buffer.ReadInt(2);
            KilledCount = buffer.ReadInt(32);
            XPMultiplier = buffer.ReadFloat32();
            TotalTime = buffer.ReadInt(32);
            Expired = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(2, BonusType);
            buffer.WriteInt(32, KilledCount);
            buffer.WriteFloat32(XPMultiplier);
            buffer.WriteInt(32, TotalTime);
            buffer.WriteBool(Expired);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("KillCounterUpdateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("BonusType: 0x" + BonusType.ToString("X8") + " (" + BonusType + ")");
            b.Append(' ', pad); b.AppendLine("KilledCount: 0x" + KilledCount.ToString("X8") + " (" + KilledCount + ")");

            b.Append(' ', pad); b.AppendLine("TotalTime: 0x" + TotalTime.ToString("X8") + " (" + TotalTime + ")");
            b.Append(' ', pad); b.AppendLine("Expired: " + (Expired ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

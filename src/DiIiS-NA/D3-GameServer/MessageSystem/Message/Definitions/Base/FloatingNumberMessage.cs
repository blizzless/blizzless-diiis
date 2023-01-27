using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(Opcodes.FloatingNumberMessage)]
    public class FloatingNumberMessage : GameMessage
    {
        // Enums members with a color in their name display a colored number
        // others display a localized string. FloatType.Dodged displays a
        // floating "Dodge"... crash sending (int)28 as type
        public enum FloatType
        {
            White = 0,
            WhiteCritical,
            Golden,
            Red2,              // GoldenCritical was expected
            Red,
            RedCritical,
            Dodge,
            Dodged,
            Block,
            Parry,
            Green,
            Absorbed,
            Rooted,
            Stunned,
            Blinded,
            Frozen,
            Feared,
            Charmed,
            Taunted,
            Snared,
            AttackSlowed,
            BrokeFreeze,
            BrokeBlind,
            BrokeStun,
            BrokeRoot,
            BrokeSnare,
            BrokeFear,
            Immune
        }

        public uint ActorID;
        public float Number;
        public FloatType Type;

        public FloatingNumberMessage() : base(Opcodes.FloatingNumberMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Number = buffer.ReadFloat32();
            Type = (FloatType)buffer.ReadInt(6);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteFloat32(Number);
            buffer.WriteInt(6, (int)Type);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FloatingNumberMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Number: " + Number.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Type: 0x" + ((int)Type).ToString("X8") + " (" + Type + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

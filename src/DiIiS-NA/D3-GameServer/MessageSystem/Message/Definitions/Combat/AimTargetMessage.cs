using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Combat
{
    [Message(Opcodes.AimTargetMessage)]
    public class AimTargetMessage : GameMessage
    {
        public int ann;
        public int Type;
        public int Target;
        public WorldPlace Place;

        public override void Parse(GameBitBuffer buffer)
        {
            ann = buffer.ReadInt(32);
            Type = buffer.ReadInt(3) + (-1);
            Target = buffer.ReadInt(32);
            Place = new WorldPlace();
            Place.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ann);
            buffer.WriteInt(3, Type - (-1));
            buffer.WriteInt(32, Target);
            Place.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AimTargetMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ann: 0x" + ann.ToString("X8") + " (" + ann + ")");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("Target: 0x" + Target.ToString("X8") + " (" + Target + ")");
            Place.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

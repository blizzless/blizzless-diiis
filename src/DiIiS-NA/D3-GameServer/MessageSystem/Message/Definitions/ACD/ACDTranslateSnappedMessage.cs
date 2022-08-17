
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDTranslateSnappedMessage)]
    public class ACDTranslateSnappedMessage : GameMessage
    {
        public int ActorId;
        public Vector3D Position;
        public float /* angle */ Angle;
        public bool Field3;
        public int Field4;
        public int? CameraSmoothingTime;
        public int? Field6;

        public ACDTranslateSnappedMessage() : base(Opcodes.ACDTranslateSnappedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadInt(32);
            Position = new Vector3D();
            Position.Parse(buffer);
            Angle = buffer.ReadFloat32();
            Field3 = buffer.ReadBool();
            Field4 = buffer.ReadInt(26);
            if (buffer.ReadBool())
            {
                CameraSmoothingTime = buffer.ReadInt(16);
            }
            if (buffer.ReadBool())
            {
                Field6 = buffer.ReadInt(32);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ActorId);
            Position.Encode(buffer);
            buffer.WriteFloat32(Angle);
            buffer.WriteBool(Field3);
            buffer.WriteInt(26, Field4);
            buffer.WriteBool(CameraSmoothingTime.HasValue);
            if (CameraSmoothingTime.HasValue)
            {
                buffer.WriteInt(16, CameraSmoothingTime.Value);
            }
            buffer.WriteBool(Field6.HasValue);
            if (Field6.HasValue)
            {
                buffer.WriteInt(32, Field6.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDTranslateSnappedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + ActorId.ToString("X8"));
            Position.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Angle: " + Angle.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Field3: " + (Field3 ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Field4: 0x" + Field4.ToString("X8") + " (" + Field4 + ")");
            if (CameraSmoothingTime.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("CameraSmoothingTime.Value: 0x" + CameraSmoothingTime.Value.ToString("X8") + " (" + CameraSmoothingTime.Value + ")");
            }
            if (Field6.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Field6.Value: 0x" + Field6.Value.ToString("X8") + " (" + Field6.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

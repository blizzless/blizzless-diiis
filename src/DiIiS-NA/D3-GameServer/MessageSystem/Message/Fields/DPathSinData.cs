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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class DPathSinData
    {
        public float SinIncPerTick;
        public float SinIncAccel;
        public float LateralMaxDistance;
        public float OOLateralDistanceToScale;
        public float LateralStartDistance;
        public float Speed;
        public int annOwner;

        public void Parse(GameBitBuffer buffer)
        {
            SinIncPerTick = buffer.ReadFloat32();
            SinIncAccel = buffer.ReadFloat32();
            LateralMaxDistance = buffer.ReadFloat32();
            OOLateralDistanceToScale = buffer.ReadFloat32();
            LateralStartDistance = buffer.ReadFloat32();
            Speed = buffer.ReadFloat32();
            annOwner = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(SinIncPerTick);
            buffer.WriteFloat32(SinIncAccel);
            buffer.WriteFloat32(LateralMaxDistance);
            buffer.WriteFloat32(OOLateralDistanceToScale);
            buffer.WriteFloat32(LateralStartDistance);
            buffer.WriteFloat32(Speed);
            buffer.WriteInt(32, annOwner);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DPathSinData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("SinIncPerTick: " + SinIncPerTick.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("SinIncAccel: " + SinIncAccel.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("LateralMaxDistance: " + LateralMaxDistance.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("OOLateralDistanceToScale: " + OOLateralDistanceToScale.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("LateralStartDistance: " + LateralStartDistance.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Speed: " + Speed.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("annOwner: 0x" + annOwner.ToString("X8") + " (" + annOwner + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

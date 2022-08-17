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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class WorldLocationMessageData
    {
        public float Scale;
        public PRTransform Transform;
        public uint WorldID;

        public void Parse(GameBitBuffer buffer)
        {
            Scale = buffer.ReadFloat32();
            Transform = new PRTransform();
            Transform.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Scale);
            Transform.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldLocationMessageData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Scale: " + Scale.ToString("G"));
            Transform.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

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
    public class WorldPlace
    {
        public Vector3D Position;
        public uint WorldID; // World's DynamicID

        public void Parse(GameBitBuffer buffer)
        {
            Position = new Vector3D();
            Position.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            Position.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldPlace:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            //Position.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

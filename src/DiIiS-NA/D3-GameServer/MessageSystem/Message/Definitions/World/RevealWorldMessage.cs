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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World
{
    [Message(Opcodes.RevealWorldMessage)]
    public class RevealWorldMessage : GameMessage
    {
        public uint WorldID; // World's DynamicID
        public int /* sno */ WorldSNO;
        public int OriginX;
        public int OriginY;
        public int StitchSizeInFeetX;
        public int StitchSizeInFeetY;
        public int WorldSizeInFeetX;
        public int WorldSizeInFeetY;
        public int /* sno */ snoDungeonFinderSourceWorld;

        public RevealWorldMessage() : base(Opcodes.RevealWorldMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
            WorldSNO = buffer.ReadInt(32);
            OriginX = buffer.ReadInt(32);
            OriginY = buffer.ReadInt(32);
            StitchSizeInFeetX = buffer.ReadInt(32);
            StitchSizeInFeetY = buffer.ReadInt(32);
            WorldSizeInFeetX = buffer.ReadInt(32);
            WorldSizeInFeetY = buffer.ReadInt(32);
            snoDungeonFinderSourceWorld = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
            buffer.WriteInt(32, WorldSNO);
            buffer.WriteInt(32, OriginX);
            buffer.WriteInt(32, OriginY);
            buffer.WriteInt(32, StitchSizeInFeetX);
            buffer.WriteInt(32, StitchSizeInFeetY);
            buffer.WriteInt(32, WorldSizeInFeetX);
            buffer.WriteInt(32, WorldSizeInFeetY);
            buffer.WriteInt(32, snoDungeonFinderSourceWorld);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RevealWorldMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("WorldSNO: 0x" + WorldSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("OriginX: 0x" + OriginX.ToString("X8") + " (" + OriginX + ")");
            b.Append(' ', pad); b.AppendLine("OriginY: 0x" + OriginY.ToString("X8") + " (" + OriginY + ")");
            b.Append(' ', pad); b.AppendLine("StitchSizeInFeetX: 0x" + StitchSizeInFeetX.ToString("X8") + " (" + StitchSizeInFeetX + ")");
            b.Append(' ', pad); b.AppendLine("StitchSizeInFeetY: 0x" + StitchSizeInFeetY.ToString("X8") + " (" + StitchSizeInFeetY + ")");
            b.Append(' ', pad); b.AppendLine("WorldSizeInFeetX: 0x" + WorldSizeInFeetX.ToString("X8") + " (" + WorldSizeInFeetX + ")");
            b.Append(' ', pad); b.AppendLine("WorldSizeInFeetY: 0x" + WorldSizeInFeetY.ToString("X8") + " (" + WorldSizeInFeetY + ")");
            b.Append(' ', pad); b.AppendLine("snoDungeonFinderSourceWorld: 0x" + snoDungeonFinderSourceWorld.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

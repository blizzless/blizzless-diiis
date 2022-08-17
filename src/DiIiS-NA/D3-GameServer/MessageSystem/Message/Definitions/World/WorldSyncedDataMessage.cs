//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
    [Message(Opcodes.WorldSyncedDataMessage)]
    public class WorldSyncedDataMessage : GameMessage
    {
        public uint WorldID; // World's DynamicID
        public WorldSyncedData SyncedData;

        public WorldSyncedDataMessage() : base(Opcodes.WorldSyncedDataMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
            SyncedData = new WorldSyncedData();
            SyncedData.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
            SyncedData.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldSyncedDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad++);
            SyncedData.AsText(b, pad);
            b.Append(' ', pad++);
            b.AppendLine("}");
        }


    }
}

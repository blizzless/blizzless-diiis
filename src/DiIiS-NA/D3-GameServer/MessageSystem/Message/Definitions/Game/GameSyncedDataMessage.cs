using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.GameSyncedDataMessage)]
    public class GameSyncedDataMessage : GameMessage
    {
        public GameSyncedData SyncedData;

        public GameSyncedDataMessage() : base(Opcodes.GameSyncedDataMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SyncedData = new GameSyncedData();
            SyncedData.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            SyncedData.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GameSyncedDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            SyncedData.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

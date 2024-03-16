using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection
{
    [Message(Opcodes.ConnectionEstablishedMessage)]
    public class ConnectionEstablishedMessage : GameMessage
    {
        public int PlayerIndex;
        public int AnimSyncedSeed;
        public int SNOPackHash;

        public ConnectionEstablishedMessage() : base(Opcodes.ConnectionEstablishedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(3);
            AnimSyncedSeed = buffer.ReadInt(32);
            SNOPackHash = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerIndex);
            buffer.WriteInt(32, AnimSyncedSeed);
            buffer.WriteInt(32, SNOPackHash);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ConnectionEstablishedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("AnimSyncedSeed: 0x" + AnimSyncedSeed.ToString("X8") + " (" + AnimSyncedSeed + ")");
            b.Append(' ', pad); b.AppendLine("SNOPackHash: 0x" + SNOPackHash.ToString("X8") + " (" + SNOPackHash + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

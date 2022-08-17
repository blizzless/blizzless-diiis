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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter
{
    [Message(Opcodes.RiftJoinMessage)]
    public class RiftJoinMessage : GameMessage
    {
        public int PlayerIndex;
        public int RiftTier;
        public int RiftStartServerTime;

        public RiftJoinMessage() : base(Opcodes.RiftJoinMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(32);
            RiftTier = buffer.ReadInt(32);
            RiftStartServerTime = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PlayerIndex);
            buffer.WriteInt(32, RiftTier);
            buffer.WriteInt(32, RiftStartServerTime);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RiftJoinMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("RiftTier: 0x" + RiftTier.ToString("X8") + " (" + RiftTier + ")");
            b.Append(' ', pad); b.AppendLine("RiftStartServerTime: 0x" + RiftStartServerTime.ToString("X8") + " (" + RiftStartServerTime + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

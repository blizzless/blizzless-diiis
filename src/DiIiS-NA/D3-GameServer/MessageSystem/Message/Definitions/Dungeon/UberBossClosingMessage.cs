using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.D3_GameServer.MessageSystem.Message.Definitions.Dungeon
{
    [Message(Opcodes.UberBossClosingMessage)]
    public class UberBossClosingMessage : GameMessage
    {
        public int field0;
        public int field1;

        public UberBossClosingMessage() : base(Opcodes.UberBossClosingMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            field0 = buffer.ReadInt(32);
            field1 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, field0);
            buffer.WriteInt(32, field1);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SetDungeonJoinMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("field0: 0x" + field0.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("field1: 0x" + field1.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

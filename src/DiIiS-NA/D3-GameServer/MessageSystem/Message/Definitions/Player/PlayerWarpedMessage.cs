using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerWarpedMessage)]
    public class PlayerWarpedMessage : GameMessage
    {
        public int WarpReason;
        public float WarpFadeInSecods;

        public PlayerWarpedMessage() : base(Opcodes.PlayerWarpedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WarpReason = buffer.ReadInt(5);
            WarpFadeInSecods = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(5, WarpReason);
            buffer.WriteFloat32(WarpFadeInSecods);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerWarpedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WarpReason: 0x" + WarpReason.ToString("X8") + " (" + WarpReason + ")");
            b.Append(' ', pad); b.AppendLine("WarpFadeInSecods: " + WarpFadeInSecods.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

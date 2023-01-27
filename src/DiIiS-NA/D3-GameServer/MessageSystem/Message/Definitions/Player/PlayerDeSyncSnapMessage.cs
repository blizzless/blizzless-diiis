using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerDeSyncSnapMessage)]
    public class PlayerDeSyncSnapMessage : GameMessage
    {
        public WorldPlace PlaceSnap;
        public int Reason;
        public int /* sno */ SnoPowerCurrent;
        public int Field3;

        public PlayerDeSyncSnapMessage() : base(Opcodes.PlayerDeSyncSnapMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlaceSnap = new WorldPlace();
            PlaceSnap.Parse(buffer);
            Reason = buffer.ReadInt(2);
            SnoPowerCurrent = buffer.ReadInt(32);
            Field3 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            PlaceSnap.Encode(buffer);
            buffer.WriteInt(2, Reason);
            buffer.WriteInt(32, SnoPowerCurrent);
            buffer.WriteInt(32, Field3);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerDeSyncSnapMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            PlaceSnap.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Reason: " + Reason.ToString());
            b.Append(' ', pad); b.AppendLine("SnoPowerCurrent: 0x" + SnoPowerCurrent.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + Field3.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

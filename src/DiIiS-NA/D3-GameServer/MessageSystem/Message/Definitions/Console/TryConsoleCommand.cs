using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Console
{
    [Message(new[] { Opcodes.ClientTryConsoleCommand, Opcodes.ServerTryConsoleCommand })]
    public class TryConsoleCommand : GameMessage
    {
        public string Message;
        public int PlayerIndex;
        public WorldPlace PlaceTarget;
        public int annTarget;
        public int Flags;

        public override void Parse(GameBitBuffer buffer)
        {
            Message = buffer.ReadCharArray(1024);
            PlayerIndex = buffer.ReadInt(4) + (-1);
            PlaceTarget = new WorldPlace();
            PlaceTarget.Parse(buffer);
            annTarget = buffer.ReadInt(32);
            Flags = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteCharArray(1024, Message);
            buffer.WriteInt(4, PlayerIndex - (-1));
            PlaceTarget.Encode(buffer);
            buffer.WriteInt(32, annTarget);
            buffer.WriteInt(32, Flags);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TryConsoleCommand:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Message: \"" + Message + "\"");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            PlaceTarget.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("annTarget: 0x" + annTarget.ToString("X8") + " (" + annTarget + ")");
            b.Append(' ', pad); b.AppendLine("Flags: 0x" + Flags.ToString("X8") + " (" + Flags + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

using DiIiS_NA.GameServer.ClientSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection
{
    [Message(new[] { Opcodes.LogoutContextMessage, Opcodes.LogoutCancel })]
    public class LogoutContextMessage : GameMessage, ISelfHandler
    {
        public bool Field0;

        public void Handle(GameClient client)
        {
            client.IsLoggingOut = !client.IsLoggingOut;

            if (client.IsLoggingOut)
            {
                client.SendMessage(new LogoutTickTimeMessage()
                {
                    Field0 = false, // true - logout with party?
                    Ticks = 100, // delay 1, make this equal to 0 for instant logout
                    Field2 = 0, // delay 2
                });
            }
        }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("LogoutContextMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Mail
{
    [Message(Opcodes.MailSentResponseMessage)]
    public class MailSentResponseMessage : GameMessage
    {
        public long Mail;
        public int Err;

        public override void Parse(GameBitBuffer buffer)
        {
            Mail = buffer.ReadInt64(64);
            Err = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt64(64, Mail);
            buffer.WriteInt(32, Err);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

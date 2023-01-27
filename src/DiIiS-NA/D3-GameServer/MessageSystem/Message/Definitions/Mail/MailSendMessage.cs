using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Mail
{
    [Message(Opcodes.MailSendMessage)]
    public class MailSendMessage : GameMessage
    {
        public int ToAccount;


        public override void Parse(GameBitBuffer buffer)
        {
            ToAccount = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, ToAccount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

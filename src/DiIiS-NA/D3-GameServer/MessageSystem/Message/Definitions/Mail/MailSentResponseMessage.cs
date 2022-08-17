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

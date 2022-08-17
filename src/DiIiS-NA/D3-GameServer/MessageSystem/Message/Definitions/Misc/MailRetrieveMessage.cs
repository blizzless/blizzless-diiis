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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
	[Message(Opcodes.MailContentsMessage, Consumers.Player)]
	public class MailRetrieveMessage : GameMessage
	{
		public long MailId;

		public MailRetrieveMessage() : base(Opcodes.MailContentsMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			MailId = buffer.ReadInt64(64);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt64(64, MailId);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("MailRetrieveMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("MailId: 0x" + MailId.ToString("X16") + " (" + MailId + ")");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}


	}

}

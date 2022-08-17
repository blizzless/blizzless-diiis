//Blizzless Project 2022 
using D3.GameMessage;
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
	[Message(new Opcodes[] { Opcodes.MailDigestMessage })]
	public class MailDigestMessage : GameMessage
	{

		public MailContents MailContents;

		public MailDigestMessage() : base(Opcodes.MailDigestMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			MailContents = MailContents.ParseFrom(buffer.ReadBlob(32));
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteBlob(32, MailContents.ToByteArray());
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("MailDigestMessage:");
			b.Append(' ', pad++);
			b.Append(MailContents.ToString());
		}

	}
}

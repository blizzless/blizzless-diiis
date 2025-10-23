using D3.GameMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

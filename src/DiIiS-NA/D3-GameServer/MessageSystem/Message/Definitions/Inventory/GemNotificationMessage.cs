using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
	[Message(Opcodes.GemNotificationMessage)]
	public class GemNotificationMessage : GameMessage
	{

		public GemNotificationMessage() : base(Opcodes.GemNotificationMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			// do not return back a not-implemented exception! /raist.
		}

		public override void Encode(GameBitBuffer buffer)
		{
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("GemNotificationMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}

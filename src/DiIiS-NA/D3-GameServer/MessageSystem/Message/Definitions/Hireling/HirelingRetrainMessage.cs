using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Hireling
{
	[Message(Opcodes.HirelingRetrainMessage, Consumers.Player)]
	public class HirelingRetrainMessage : GameMessage
	{
		public HirelingRetrainMessage()
			: base(Opcodes.HirelingRetrainMessage)
		{
		}

		public override void Parse(GameBitBuffer buffer)
		{
		}

		public override void Encode(GameBitBuffer buffer)
		{
			throw new NotImplementedException();
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("HirelingRetrainMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}

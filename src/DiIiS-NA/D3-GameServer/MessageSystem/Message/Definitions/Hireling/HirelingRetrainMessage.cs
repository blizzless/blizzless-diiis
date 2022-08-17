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

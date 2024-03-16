using D3.GameMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
	[Message(Opcodes.TutorialShownMessage, Consumers.Player)]
	public class TutorialShownMessage : GameMessage
	{
		public int SNOTutorial;

		public override void Parse(GameBitBuffer buffer)
		{
			SNOTutorial = buffer.ReadInt(32);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, SNOTutorial);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("TutorialShownMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("SNOTutorial: 0x" + SNOTutorial.ToString("X8"));
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}

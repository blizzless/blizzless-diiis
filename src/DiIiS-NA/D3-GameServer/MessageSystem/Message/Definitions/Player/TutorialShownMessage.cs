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

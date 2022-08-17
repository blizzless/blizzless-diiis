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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
	[Message(Opcodes.PauseGameMessage, Consumers.Game)]
	public class PauseGameMessage : GameMessage
	{
		public bool Field0;

		public override void Parse(GameBitBuffer buffer)
		{
			Field0 = buffer.ReadBool();
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteBool(Field0);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("PauseGameMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
			b.Append(' ', --pad);
			b.AppendLine("}");
		}


	}

	[Message(Opcodes.FreezeGameMessage)]
	public class FreezeGameMessage : GameMessage
	{
		public bool Field0;

		public FreezeGameMessage() : base(Opcodes.FreezeGameMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			Field0 = buffer.ReadBool();
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteBool(Field0);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("FreezeGameMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("Field0: " + (Field0 ? "true" : "false"));
			b.Append(' ', --pad);
			b.AppendLine("}");
		}


	}
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.TrySalvageMessage, Consumers.Inventory)]
	public class TrySalvageMessage : GameMessage
	{
		public uint ActorID; // Id of the target

		public TrySalvageMessage() : base(Opcodes.TrySalvageMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			ActorID = buffer.ReadUInt(32);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteUInt(32, ActorID);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("RequestUseNephalemCubeMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
			b.Append(' ', --pad);
			b.AppendLine("}");
		}

	}
}

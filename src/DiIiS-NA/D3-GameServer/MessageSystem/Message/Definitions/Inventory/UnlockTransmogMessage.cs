using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Inventory
{
	[Message(Opcodes.UnlockTransmogMessage)]
	public class UnlockTransmogMessage : GameMessage
	{
		public int TransmogGBID;

		public UnlockTransmogMessage() : base(Opcodes.UnlockTransmogMessage) { }

		public override void Parse(GameBitBuffer buffer)
		{
			TransmogGBID = buffer.ReadInt(32);
		}

		public override void Encode(GameBitBuffer buffer)
		{
			buffer.WriteInt(32, TransmogGBID);
		}

		public override void AsText(StringBuilder b, int pad)
		{
			b.Append(' ', pad);
			b.AppendLine("UnlockTransmogMessage:");
			b.Append(' ', pad++);
			b.AppendLine("{");
			b.Append(' ', pad); b.AppendLine("TransmogGBID: 0x" + TransmogGBID.ToString("X8"));
			b.Append(' ', --pad);
			b.AppendLine("}");
		}
	}
}
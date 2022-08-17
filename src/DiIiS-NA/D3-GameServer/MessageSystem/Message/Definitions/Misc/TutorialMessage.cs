//Blizzless Project 2022 
using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.TutorialMessage)]
    public class TutorialMessage : GameMessage
    {
        public D3.GameMessage.TutorialMessage TutorialMessageDefinition;

        public TutorialMessage() : base(Opcodes.TutorialMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            TutorialMessageDefinition = D3.GameMessage.TutorialMessage.ParseFrom(buffer.ReadBlob(32));
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, TutorialMessageDefinition.ToByteArray());
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TutorialMessage:");
            b.Append(' ', pad++);
            b.Append(TutorialMessageDefinition.ToString());
        }
    }
}

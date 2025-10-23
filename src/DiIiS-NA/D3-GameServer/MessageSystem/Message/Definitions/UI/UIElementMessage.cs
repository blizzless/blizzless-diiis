using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.UI
{
    [Message(Opcodes.UIElementMessage)]
    public class UIElementMessage : GameMessage
    {
        public int Element;
        public bool Action;
        public UIElementMessage() : base(Opcodes.UIElementMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Element = buffer.ReadInt(5) + (-1);
            Action = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(5, Element - (-1));
            buffer.WriteBool(Action);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("UIElementMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Element: 0x" + Element.ToString("X8") + " (" + Element + ")");
            b.Append(' ', pad); b.AppendLine("Action: " + (Action ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

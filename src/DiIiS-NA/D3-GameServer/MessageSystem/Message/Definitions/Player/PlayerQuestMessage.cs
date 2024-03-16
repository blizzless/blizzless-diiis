using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] { Opcodes.PlayerQuestMessage, Opcodes.PlayerQuestMessage1 })]
    public class PlayerQuestMessage : GameMessage
    {
        public int PlayerIndex;
        public int /* sno */ SnoQuest;
        public int /* sno */ SnoLevelArea;

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(4) + (-1);
            SnoQuest = buffer.ReadInt(32);
            SnoLevelArea = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(4, PlayerIndex - (-1));
            buffer.WriteInt(32, SnoQuest);
            buffer.WriteInt(32, SnoLevelArea);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerQuestMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + SnoQuest.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + SnoLevelArea.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

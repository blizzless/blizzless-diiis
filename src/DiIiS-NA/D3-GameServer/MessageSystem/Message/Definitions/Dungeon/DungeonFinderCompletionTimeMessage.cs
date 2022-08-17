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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Dungeon
{
    [Message(Opcodes.DungeonFinderCompletionTimeMessage)]
    public class DungeonFinderCompletionTimeMessage : GameMessage
    {
        public int CompletitionTimeMessage;
        public int PlayerIndex;

        public DungeonFinderCompletionTimeMessage() : base(Opcodes.DungeonFinderCompletionTimeMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            CompletitionTimeMessage = buffer.ReadInt(32);
            PlayerIndex = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, CompletitionTimeMessage);
            buffer.WriteInt(32, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("DungeonFinderCompletionTimeMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("CompletitionTimeMessage: 0x" + CompletitionTimeMessage.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

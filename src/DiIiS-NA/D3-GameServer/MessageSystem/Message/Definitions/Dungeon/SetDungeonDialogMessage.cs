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
    [Message(Opcodes.SetDungeonJoinMessage)]
    public class SetDungeonJoinMessage : GameMessage
    {
        public int PlayerIndex;
        public int LabelDescription;
        public int LabelTitle;

        public SetDungeonJoinMessage() : base(Opcodes.SetDungeonJoinMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(32);
            LabelDescription = buffer.ReadInt(32);
            LabelTitle = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, PlayerIndex);
            buffer.WriteInt(32, LabelDescription);
            buffer.WriteInt(32, LabelTitle);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SetDungeonJoinMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("LabelDescription: 0x" + LabelDescription.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("LabelTitle: 0x" + LabelTitle.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

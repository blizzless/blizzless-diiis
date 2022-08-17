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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Team
{
    [Message(Opcodes.RevealTeamMessage)]
    public class RevealTeamMessage : GameMessage
    {
        public int Team;
        public int TeamFlags;
        public int TeamColoring;

        public RevealTeamMessage() : base(Opcodes.RevealTeamMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Team = buffer.ReadInt(5) + (-1);
            TeamFlags = buffer.ReadInt(2);
            TeamColoring = buffer.ReadInt(2) + (-1);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(5, Team - (-1));
            buffer.WriteInt(2, TeamFlags);
            buffer.WriteInt(2, TeamColoring - (-1));
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RevealTeamMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Team: 0x" + Team.ToString("X8") + " (" + Team + ")");
            b.Append(' ', pad); b.AppendLine("TeamFlags: 0x" + TeamFlags.ToString("X8") + " (" + TeamFlags + ")");
            b.Append(' ', pad); b.AppendLine("TeamColoring: 0x" + TeamColoring.ToString("X8") + " (" + TeamColoring + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

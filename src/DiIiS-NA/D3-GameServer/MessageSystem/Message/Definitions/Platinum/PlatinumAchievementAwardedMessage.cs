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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Platinum
{
    [Message(Opcodes.PlatinumAchievementAwardedMessage)]
    public class PlatinumAchievementAwardedMessage : GameMessage
    {
        public long CurrentPlatinum;
        public long PlatinumIncrement;
        public ulong idAchievement;

        public PlatinumAchievementAwardedMessage() : base(Opcodes.PlatinumAchievementAwardedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            CurrentPlatinum = buffer.ReadInt64(64);
            PlatinumIncrement = buffer.ReadInt64(64);
            idAchievement = buffer.ReadUInt64(64);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt64(64, CurrentPlatinum);
            buffer.WriteInt64(64, PlatinumIncrement);
            buffer.WriteUInt64(64, idAchievement);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlatinumAchievementAwardedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("CurrentPlatinum: 0x" + CurrentPlatinum.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("PlatinumIncrement: 0x" + PlatinumIncrement.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("idAchievement: 0x" + idAchievement.ToString("X16"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

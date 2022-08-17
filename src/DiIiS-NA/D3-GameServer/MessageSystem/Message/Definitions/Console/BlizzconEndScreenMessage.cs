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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Console
{
    [Message(new[] { Opcodes.BlizzconRiftEndedMessage })]
    public class BlizzconEndScreenMessage : GameMessage
    {
        public int RewardTier;
        public uint ann;

        public BlizzconEndScreenMessage() : base(Opcodes.BlizzconRiftEndedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            RewardTier = buffer.ReadInt(32);
            ann = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, RewardTier);
            buffer.WriteUInt(32, ann);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BlizzconEndScreenMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("RewardTier: 0x" + RewardTier.ToString("X8") + " (" + RewardTier + ")");
            b.Append(' ', pad); b.AppendLine("ann: 0x" + ann.ToString("X8") + " (" + ann + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

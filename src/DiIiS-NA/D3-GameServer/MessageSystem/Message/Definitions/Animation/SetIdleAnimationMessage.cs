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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation
{
    [Message(Opcodes.SetIdleAnimationMessage)]
    public class SetIdleAnimationMessage : GameMessage
    {
        public uint ActorID;
        public int AnimationSNO;

        public SetIdleAnimationMessage() : base(Opcodes.SetIdleAnimationMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            AnimationSNO = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(32, AnimationSNO);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SetIdleAnimationMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("AnimationSNO: 0x" + AnimationSNO.ToString("X8") + " (" + AnimationSNO + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

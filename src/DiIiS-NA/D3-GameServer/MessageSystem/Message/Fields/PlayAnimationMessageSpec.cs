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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class PlayAnimationMessageSpec
    {
        public int Duration;
        public int AnimationSNO;
        public int AnimationTag;
        public int PermutationIndex;
        public float Speed;

        public void Parse(GameBitBuffer buffer)
        {
            Duration = buffer.ReadInt(32);
            AnimationSNO = buffer.ReadInt(32);
            AnimationTag = buffer.ReadInt(32);
            PermutationIndex = buffer.ReadInt(32);
            Speed = buffer.ReadFloat32();
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Duration);
            buffer.WriteInt(32, AnimationSNO);
            buffer.WriteInt(32, AnimationTag);
            buffer.WriteInt(32, PermutationIndex);
            buffer.WriteFloat32(Speed);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayAnimationMessageSpec:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Duration: 0x" + Duration.ToString("X8") + " (" + Duration + " ticks)");
            b.Append(' ', pad);
            b.AppendLine("AnimationSNO: 0x" + AnimationSNO.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("AnimationTag: 0x" + AnimationTag.ToString("X8") + " (" + AnimationTag + ")");
            b.Append(' ', pad);
            b.AppendLine("PermutationIndex: 0x" + PermutationIndex.ToString("X8") + " (" + PermutationIndex + ")");
            b.Append(' ', pad);
            b.AppendLine("Speed: " + Speed.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Animation
{
    [Message(Opcodes.PlayAnimationMessage)]
    public class PlayAnimationMessage : GameMessage
    {
        public uint ActorID;
        public int AnimReason;
        public float UnitAniimStartTime;
        // MaxLength = 3
        public PlayAnimationMessageSpec[] tAnim;

        public PlayAnimationMessage() : base(Opcodes.PlayAnimationMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            AnimReason = buffer.ReadInt(4);
            UnitAniimStartTime = buffer.ReadFloat32();
            tAnim = new PlayAnimationMessageSpec[buffer.ReadInt(2)];
            for (int i = 0; i < tAnim.Length; i++) { tAnim[i] = new PlayAnimationMessageSpec(); tAnim[i].Parse(buffer); }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(4, AnimReason);
            buffer.WriteFloat32(UnitAniimStartTime);
            buffer.WriteInt(2, tAnim.Length);
            for (int i = 0; i < tAnim.Length; i++) { tAnim[i].Encode(buffer); }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayAnimationMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("AniimReason: 0x" + AnimReason.ToString("X8") + " (" + AnimReason + ")");
            b.Append(' ', pad); b.AppendLine("UnitAniimStartTime: " + UnitAniimStartTime.ToString("G"));
            b.Append(' ', pad); b.AppendLine("tAnim:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < tAnim.Length; i++) { tAnim[i].AsText(b, pad + 1); b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

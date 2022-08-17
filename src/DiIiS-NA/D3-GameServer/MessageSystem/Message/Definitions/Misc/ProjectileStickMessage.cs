//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.ProjectileStickMessage)]
    public class ProjectileStickMessage : GameMessage
    {
        public Vector3D SourceLoc;
        public int annTarget;
        public int /* sno */ snoActorAttachment;

        public override void Parse(GameBitBuffer buffer)
        {
            SourceLoc = new Vector3D();
            SourceLoc.Parse(buffer);
            annTarget = buffer.ReadInt(32);
            snoActorAttachment = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            SourceLoc.Encode(buffer);
            buffer.WriteInt(32, annTarget);
            buffer.WriteInt(32, snoActorAttachment);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ProjectileStickMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            SourceLoc.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("annTarget: 0x" + annTarget.ToString("X8") + " (" + annTarget + ")");
            b.Append(' ', pad); b.AppendLine("snoActorAttachment: 0x" + snoActorAttachment.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

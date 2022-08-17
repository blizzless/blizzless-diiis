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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Camera
{
    [Message(Opcodes.CameraCriptedSequenceStartMessage)]
    public class CameraCriptedSequenceStartMessage : GameMessage
    {
        public bool Activate;
        public CameraCriptedSequenceStartMessage() : base(Opcodes.CameraCriptedSequenceStartMessage)
        { 
        
        }
        public override void Parse(GameBitBuffer buffer)
        {
            Activate = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(Activate);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("BossZoomMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

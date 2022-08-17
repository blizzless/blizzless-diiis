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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    
    [Message(new[] {
        Opcodes.FloatDataMessage
        ,Opcodes.DungeonFinderProgressMessage
        ,Opcodes.DunggeonFinderProgressGlyphPickUp
        ,Opcodes.FloatDataMessage3
        })]
    public class FloatDataMessage : GameMessage
    {
        public float Field0;
        public FloatDataMessage(Opcodes opcode) : base(opcode) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadFloat32();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteFloat32(Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FloatDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); 
            b.AppendLine("Field0: 0x" + Field0.ToString("F") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

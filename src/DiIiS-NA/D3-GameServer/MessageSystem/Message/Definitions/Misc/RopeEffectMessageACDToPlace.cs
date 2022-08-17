//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
    [Message(Opcodes.RopeEffectMessageACDToPlace)]
    public class RopeEffectMessageACDToPlace : GameMessage
    {
        public int /* sno */ RopeSNO;
        public int StartSourceActorId;
        public int Field2;  // always seem to be 4
        public WorldPlace EndPosition;
        public bool Field4;

        public RopeEffectMessageACDToPlace() : base(Opcodes.RopeEffectMessageACDToPlace) { }

        public override void Parse(GameBitBuffer buffer)
        {
            RopeSNO = buffer.ReadInt(32);
            StartSourceActorId = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(5);
            EndPosition = new WorldPlace();
            EndPosition.Parse(buffer);
            Field4 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, RopeSNO);
            buffer.WriteInt(32, StartSourceActorId);
            buffer.WriteInt(5, Field2);
            EndPosition.Encode(buffer);
            buffer.WriteBool(Field4);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("RopeEffectMessageACDToPlace:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("RopeSNO: 0x" + RopeSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("StartSourceActorId: 0x" + StartSourceActorId.ToString("X8") + " (" + StartSourceActorId + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + Field2.ToString("X8") + " (" + Field2 + ")");
            EndPosition.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

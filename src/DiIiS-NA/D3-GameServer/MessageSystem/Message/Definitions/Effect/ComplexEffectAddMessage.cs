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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect
{
    [Message(Opcodes.ComplexEffectAddMessage)]
    public class ComplexEffectAddMessage : GameMessage
    {
        public int EffectId;
        public int Type;  // 0=efg, 1=efg, 2=rope
        public int /* sno */ EffectSNO;
        public int SourceActorId;
        public int TargetActorId;
        public int Param1;  // 0=efg, 4=rope1, 3=rope2
        public int Param2;  // 0=efg, 1=rope1, 3=rope2
        public bool IgroneOwnerAlpha; 

        public ComplexEffectAddMessage() : base(Opcodes.ComplexEffectAddMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            EffectId = buffer.ReadInt(32);
            Type = buffer.ReadInt(32);
            EffectSNO = buffer.ReadInt(32);
            SourceActorId = buffer.ReadInt(32);
            TargetActorId = buffer.ReadInt(32);
            Param1 = buffer.ReadInt(32);
            Param2 = buffer.ReadInt(32);
            IgroneOwnerAlpha = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, EffectId);
            buffer.WriteInt(32, Type);
            buffer.WriteInt(32, EffectSNO);
            buffer.WriteInt(32, SourceActorId);
            buffer.WriteInt(32, TargetActorId);
            buffer.WriteInt(32, Param1);
            buffer.WriteInt(32, Param2);
            buffer.WriteBool(IgroneOwnerAlpha);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ComplexEffectAddMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("EffectId: 0x" + EffectId.ToString("X8") + " (" + EffectId + ")");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("EffectSNO: 0x" + EffectSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("SourceActorId: 0x" + SourceActorId.ToString("X8") + " (" + SourceActorId + ")");
            b.Append(' ', pad); b.AppendLine("TargetActorId: 0x" + TargetActorId.ToString("X8") + " (" + TargetActorId + ")");
            b.Append(' ', pad); b.AppendLine("Param1: 0x" + Param1.ToString("X8") + " (" + Param1 + ")");
            b.Append(' ', pad); b.AppendLine("Param2: 0x" + Param2.ToString("X8") + " (" + Param2 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

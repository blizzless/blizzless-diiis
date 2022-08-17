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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Pet
{
    [Message(Opcodes.PetMessage)]
    public class PetMessage : GameMessage
    {
        public int Owner;
        public int Index;
        public uint PetId;
        public int Type;

        public PetMessage()
            : base(Opcodes.PetMessage)
        {
        }

        public override void Parse(GameBitBuffer buffer)
        {
            //Field0 = buffer.ReadInt(3);
            //Field1 = buffer.ReadInt(5);
            //PetId = buffer.ReadUInt(32);
            //Field3 = buffer.ReadInt(5) + (-1);
            Owner = buffer.ReadInt(3);
            Index = buffer.ReadInt(6);
            PetId = buffer.ReadUInt(32);
            Type = buffer.ReadInt(7) + (-1); 
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, Owner);
            buffer.WriteInt(6, Index);
            buffer.WriteUInt(32, PetId);
            buffer.WriteInt(7, Type - (-1));
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PetMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Owner: 0x" + Owner.ToString("X8") + " (" + Owner + ")");
            b.Append(' ', pad); b.AppendLine("Index: 0x" + Index.ToString("X8") + " (" + Index + ")");
            b.Append(' ', pad); b.AppendLine("PetId: 0x" + PetId.ToString("X8") + " (" + PetId + ")");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Attribute
{
    [Message(Opcodes.AttributeSetValueMessage)]
    public class AttributeSetValueMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID
        public NetAttributeKeyValue Attribute;

        public AttributeSetValueMessage() : base(Opcodes.AttributeSetValueMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Attribute = new NetAttributeKeyValue();
            Attribute.Parse(buffer);
            Attribute.ParseValue(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            //if (ActorID != 0)
            {
                buffer.WriteUInt(32, ActorID);
                Attribute.Encode(buffer);
                Attribute.EncodeValue(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AttributeSetValueMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            Attribute.AsText(b, pad);
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

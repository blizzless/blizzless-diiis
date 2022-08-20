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
    [Message(Opcodes.AttributesSetValuesMessage)]
    public class AttributesSetValuesMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID        
        public NetAttributeKeyValue[] atKeyVals; // MaxLength = 15
        public int StartPosition = 0;
        public AttributesSetValuesMessage() : base(Opcodes.AttributesSetValuesMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            StartPosition = buffer.Position;
            ActorID = buffer.ReadUInt(32);
            atKeyVals = new NetAttributeKeyValue[buffer.ReadInt(4)];
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i] = new NetAttributeKeyValue(); atKeyVals[i].Parse(buffer);
                
            }
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].ParseValue(buffer); }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            //if (ActorID != 0)
            {
                buffer.WriteUInt(32, ActorID);
                buffer.WriteInt(4, atKeyVals.Length);
                for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].Encode(buffer); }
                for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].EncodeValue(buffer); }
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("AttributesSetValuesMessage, StartPosition - " + StartPosition + " :");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("atKeyVals:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < atKeyVals.Length; i++) { atKeyVals[i].AsText(b, pad + 1); b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

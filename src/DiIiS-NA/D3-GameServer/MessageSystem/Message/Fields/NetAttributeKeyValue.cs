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
    public class NetAttributeKeyValue
    {
        public int? KeyParam;
        //public int Field1;
        public GameAttribute Attribute;
        public int Int;
        public float Float;

        public void Parse(GameBitBuffer buffer)
        {

            if (buffer.ReadBool())
            {
                KeyParam = buffer.ReadInt(20);
            }
            int index = buffer.ReadInt(11);// & 0xFFF;

            Attribute = GameAttribute.Attributes[index];
        }

        public void ParseValue(GameBitBuffer buffer)
        {
            switch (Attribute.EncodingType)
            {
                case GameAttributeEncoding.Int:
                    Int = buffer.ReadInt(Attribute.BitCount);
                    break;
                case GameAttributeEncoding.IntMinMax:
                    Int = buffer.ReadInt(Attribute.BitCount) + Attribute.Min.Value;
                    break;
                case GameAttributeEncoding.Float16:
                    Float = buffer.ReadFloat16();
                    break;
                case GameAttributeEncoding.Float16Or32:
                    Float = buffer.ReadBool() ? buffer.ReadFloat16() : buffer.ReadFloat32();
                    break;
                case GameAttributeEncoding.Float32:
                    Float = buffer.ReadFloat32();
                    break;
                default:
                    throw new Exception("bad voodoo");
            }
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(KeyParam.HasValue);
            if (KeyParam.HasValue)
            {
                buffer.WriteInt(20, KeyParam.Value);
            }
            buffer.WriteInt(11, Attribute.Id);
        }

        public void EncodeValue(GameBitBuffer buffer)
        {
            switch (Attribute.EncodingType)
            {
                case GameAttributeEncoding.Int:
                    buffer.WriteInt(Attribute.BitCount, Int);
                    break;
                case GameAttributeEncoding.IntMinMax:
                    buffer.WriteInt(Attribute.BitCount, Int - Attribute.Min.Value);
                    break;
                case GameAttributeEncoding.Float16:
                    buffer.WriteFloat16(Float);
                    break;
                case GameAttributeEncoding.Float16Or32:
                    if (Float >= 65536.0f || -65536.0f >= Float)
                    {
                        buffer.WriteBool(false);
                        buffer.WriteFloat32(Float);
                    }
                    else
                    {
                        buffer.WriteBool(true);
                        buffer.WriteFloat16(Float);
                    }
                    break;
                case GameAttributeEncoding.Float32:
                    buffer.WriteFloat32(Float);
                    break;
                default:
                    throw new Exception("bad voodoo");
            }
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("NetAttributeKeyValue:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            if (KeyParam.HasValue)
            {
                b.Append(' ', pad);
                b.AppendLine("KeyParam.Value: 0x" + KeyParam.Value.ToString("X8") + " (" + KeyParam.Value + ")");
            }
            b.Append(' ', pad);
            b.Append(Attribute.Name);
            b.Append(" (" + Attribute.Id + "): ");

            if (Attribute.IsInteger)
                b.AppendLine("0x" + Int.ToString("X8") + " (" + Int + ")");
            else
                b.AppendLine(Float.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

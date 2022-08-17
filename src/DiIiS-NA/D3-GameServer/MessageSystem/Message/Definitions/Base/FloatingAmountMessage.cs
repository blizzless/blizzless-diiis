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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(Opcodes.FloatingAmountMessage)]
    public class FloatingAmountMessage : GameMessage
    {
        //Type 29 - Gold
        //Type 30 - BloodStones
        //Type 31 - Platinum
        //Type 33 - Experience
        //Type 34 - Experience + Gold
        public enum FloatType : int
        {
            Gold = 0x1D,
            BloodStone = 0x1E,
            Platinum = 0x1F,
            Experience = 20,
            ExperienceAndGold = 21
        }

        public WorldPlace Place;
        public int Amount;
        public int? OptionalGoldAmount;
        public FloatType Type;
        //public int Type;

        public FloatingAmountMessage() : base(Opcodes.FloatingAmountMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Place = new WorldPlace();
            Place.Parse(buffer);
            Amount = buffer.ReadInt(32);
            if (buffer.ReadBool())
            {
                OptionalGoldAmount = buffer.ReadInt(32);
            }
            Type = (FloatType)buffer.ReadInt(6);
            //Type = buffer.ReadInt(6);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            Place.Encode(buffer);
            buffer.WriteInt(32, Amount);
            buffer.WriteBool(OptionalGoldAmount.HasValue);
            if (OptionalGoldAmount.HasValue)
            {
                buffer.WriteInt(32, OptionalGoldAmount.Value);
            }
            buffer.WriteInt(6, (int)Type);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("FloatingAmountMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            Place.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Amount: 0x" + Amount.ToString("X8") + " (" + Amount + ")");
            if (OptionalGoldAmount.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("OptionalGoldAmount.Value: 0x" + OptionalGoldAmount.Value.ToString("X8") + " (" + OptionalGoldAmount.Value + ")");
            }
            b.Append(' ', pad); b.AppendLine("Field3: 0x" + ((int)Type).ToString("X8") + " (" + Type + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

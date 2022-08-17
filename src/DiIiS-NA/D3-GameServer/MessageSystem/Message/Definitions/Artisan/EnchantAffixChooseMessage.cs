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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.EnchantAffixChooseMessage)]
    public class EnchantAffixChooseMessage : GameMessage
    {
        public int Field0;
        public EnchantAffixChoice[] EnchantAffixChoices;
        public int EnchantAffixChoicesCount;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            EnchantAffixChoices = new EnchantAffixChoice[6];
            for (int i = 0; i < EnchantAffixChoices.Length; i++)
                EnchantAffixChoices[i].Parse(buffer);
            EnchantAffixChoicesCount = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            for (int i = 0; i < EnchantAffixChoices.Length; i++)
                EnchantAffixChoices[i].Encode(buffer);
            buffer.WriteInt(32, EnchantAffixChoicesCount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

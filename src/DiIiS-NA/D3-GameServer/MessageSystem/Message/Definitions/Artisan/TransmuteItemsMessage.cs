using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Artisan
{
    [Message(Opcodes.TransmuteItemsMessage, Consumers.Player)]
    public class TransmuteItemsMessage : GameMessage
    {
        public int[] annItems;
        public int ItemsCount;
        public int StackedItemsBitField;
        public int[] GBIDCurrencies;
        public int CurrenciesCount;

        public override void Parse(GameBitBuffer buffer)
        {
            annItems = new int[9];
            for (int i = 0; i < annItems.Length; i++)
                annItems[i] = buffer.ReadInt(32);
            ItemsCount = buffer.ReadInt(32);
            StackedItemsBitField = buffer.ReadInt(32);
            GBIDCurrencies = new int[9];
            for (int i = 0; i < GBIDCurrencies.Length; i++)
                GBIDCurrencies[i] = buffer.ReadInt(32);
            CurrenciesCount = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < annItems.Length; i++)
                buffer.WriteInt(32, annItems[i]);
            buffer.WriteInt(32, ItemsCount);
            buffer.WriteInt(32, StackedItemsBitField);
            for (int i = 0; i < GBIDCurrencies.Length; i++)
                buffer.WriteInt(32, GBIDCurrencies[i]);
            buffer.WriteInt(32, CurrenciesCount);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}

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
    [Message(Opcodes.SalvageResultsMessage)]
    public class SalvageResultsMessage : GameMessage
    {
        public int /* gbid */ gbidOriginalItem;
        public int IQLOriginalItem;
        // MaxLength = 10
        public int /* gbid */[] gbidNewItems;
        public int[] MaterialsCounts;
        public int MaterialsResults;
        public bool field5;

        public SalvageResultsMessage() : base(Opcodes.SalvageResultsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            gbidOriginalItem = buffer.ReadInt(32);
            IQLOriginalItem = buffer.ReadInt(4) + (-1);
            gbidNewItems = new int /* gbid */[4];
            for (int i = 0; i < gbidNewItems.Length; i++) gbidNewItems[i] = buffer.ReadInt(32);
            MaterialsCounts = new int[4];
            for (int i = 0; i < MaterialsCounts.Length; i++) MaterialsCounts[i] = buffer.ReadInt(32);
            MaterialsResults = buffer.ReadInt(32);
            field5 = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, gbidOriginalItem);
            buffer.WriteInt(4, IQLOriginalItem - (-1));
            for (int i = 0; i < gbidNewItems.Length; i++) buffer.WriteInt(32, gbidNewItems[i]);
            //if (MaterialsCounts != null)
            for (int i = 0; i < MaterialsCounts.Length; i++) buffer.WriteInt(32, MaterialsCounts[i]);
            buffer.WriteInt(32, MaterialsResults);
            buffer.WriteBool(field5);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SalvageResultsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("gbidOriginalItem: 0x" + gbidOriginalItem.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("IQLOriginalItem: 0x" + IQLOriginalItem.ToString("X8") + " (" + IQLOriginalItem + ")");
            b.Append(' ', pad); b.AppendLine("gbidNewItems:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < gbidNewItems.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < gbidNewItems.Length; j++, i++) { b.Append("0x" + gbidNewItems[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("MaterialsCounts:");
            b.Append(' ', pad); b.AppendLine("{");
            //for (int i = 0; i < MaterialsCounts.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < MaterialsCounts.Length; j++, i++) { b.Append("0x" + MaterialsCounts[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("MaterialsResults: 0x" + MaterialsResults.ToString("X8") + " (" + MaterialsResults + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

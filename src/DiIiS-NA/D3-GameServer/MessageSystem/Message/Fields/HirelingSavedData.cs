using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class HirelingSavedData
    {
        // MaxLength = 4
        public HirelingInfo[] HirelingInfos;
        public int ActiveHireling;
        public int AvailableHirelings;

        public void Parse(GameBitBuffer buffer)
        {
            HirelingInfos = new HirelingInfo[4];
            for (int i = 0; i < HirelingInfos.Length; i++)
            {
                HirelingInfos[i] = new HirelingInfo();
                HirelingInfos[i].Parse(buffer);
            }
            ActiveHireling = buffer.ReadInt(2);
            AvailableHirelings = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < HirelingInfos.Length; i++)
            {
                HirelingInfos[i].Encode(buffer);
            }
            buffer.WriteInt(2, ActiveHireling);
            buffer.WriteInt(32, AvailableHirelings);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("HirelingSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("HirelingInfos:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < HirelingInfos.Length; i++)
            {
                HirelingInfos[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("ActiveHireling: 0x" + ActiveHireling.ToString("X8") + " (" + ActiveHireling + ")");
            b.Append(' ', pad);
            b.AppendLine("AvailableHirelings: 0x" + AvailableHirelings.ToString("X8") + " (" + AvailableHirelings + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

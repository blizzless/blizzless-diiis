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
    public class PlayerSavedData
    {
        // MaxLength = 6
        public HotbarButtonData[] HotBarButtons;
        public byte SkillSlotEverAssigned;
        public HotbarButtonData HotBarButton;
        public int PlaytimeTotal;
        public int WaypointFlags;
        public HirelingSavedData HirelingData;
        public int TimeLastLevel;
        public LearnedLore LearnedLore;
        // MaxLength = 6
        public ActiveSkillSavedData[] ActiveSkills;
        // MaxLength = 4
        public int /* sno */[] snoTraits;
        public int[] GBIDLegendaryPowers;
        public SavePointData SavePointData;
        public int EventFlags;

        public void Parse(GameBitBuffer buffer)
        {
            HotBarButtons = new HotbarButtonData[6];
            for (int i = 0; i < HotBarButtons.Length; i++)
            {
                HotBarButtons[i] = new HotbarButtonData();
                HotBarButtons[i].Parse(buffer);
            }
            SkillSlotEverAssigned = (byte)buffer.ReadInt(8);
            HotBarButton = new HotbarButtonData();
            HotBarButton.Parse(buffer);
            PlaytimeTotal = buffer.ReadInt(32);
            WaypointFlags = buffer.ReadInt(32);
            HirelingData = new HirelingSavedData();
            HirelingData.Parse(buffer);
            TimeLastLevel = buffer.ReadInt(32);
            LearnedLore = new LearnedLore();
            LearnedLore.Parse(buffer);
            ActiveSkills = new ActiveSkillSavedData[6];
            for (int i = 0; i < ActiveSkills.Length; i++)
            {
                ActiveSkills[i] = new ActiveSkillSavedData();
                ActiveSkills[i].Parse(buffer);
            }
            snoTraits = new int /* sno */[4];
            for (int i = 0; i < snoTraits.Length; i++) snoTraits[i] = buffer.ReadInt(32);
            GBIDLegendaryPowers = new int /* gbid */[4];
            for (int i = 0; i < GBIDLegendaryPowers.Length; i++) GBIDLegendaryPowers[i] = buffer.ReadInt(32);
            SavePointData = new SavePointData();
            SavePointData.Parse(buffer);
            EventFlags = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < HotBarButtons.Length; i++)
            {
                HotBarButtons[i].Encode(buffer);
            }
            buffer.WriteInt(8, SkillSlotEverAssigned);
            HotBarButton.Encode(buffer);
            buffer.WriteInt(32, PlaytimeTotal);
            buffer.WriteInt(32, WaypointFlags);
            HirelingData.Encode(buffer);
            buffer.WriteInt(32, TimeLastLevel);
            LearnedLore.Encode(buffer);
            for (int i = 0; i < 6; i++) ActiveSkills[i].Encode(buffer);
            for (int i = 0; i < 4; i++) buffer.WriteInt(32, snoTraits[i]);
            for (int i = 0; i < 4; i++) buffer.WriteInt(32, GBIDLegendaryPowers[i]);
            SavePointData.Encode(buffer);
            buffer.WriteInt(32, EventFlags);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerSavedData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("Field0:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < HotBarButtons.Length; i++)
            {
                HotBarButtons[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            HotBarButton.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("SkillSlotEverAssigned: 0x" + SkillSlotEverAssigned.ToString("X2") + " (" + SkillSlotEverAssigned + ")");
            b.Append(' ', pad);
            b.AppendLine("PlaytimeTotal: 0x" + PlaytimeTotal.ToString("X8") + " (" + PlaytimeTotal + ")");
            b.Append(' ', pad);
            b.AppendLine("WaypointFlags: 0x" + WaypointFlags.ToString("X8") + " (" + WaypointFlags + ")");
            HirelingData.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("Field5: 0x" + TimeLastLevel.ToString("X8") + " (" + TimeLastLevel + ")");
            LearnedLore.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine("snoActiveSkills:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < ActiveSkills.Length; i++)
            {
                ActiveSkills[i].AsText(b, pad + 1);
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            b.Append(' ', pad);
            b.AppendLine("snoTraits:");
            b.Append(' ', pad);
            b.AppendLine("{");
            for (int i = 0; i < snoTraits.Length;)
            {
                b.Append(' ', pad + 1);
                for (int j = 0; j < 8 && i < snoTraits.Length; j++, i++)
                {
                    b.Append("0x" + snoTraits[i].ToString("X8") + ", ");
                }
                b.AppendLine();
            }
            b.Append(' ', pad);
            b.AppendLine("}");
            b.AppendLine();
            SavePointData.AsText(b, pad);
            b.Append(' ', pad);
            b.AppendLine();
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

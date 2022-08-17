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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] { Opcodes.PlayerLoadoutDataMessage, Opcodes.PlayerLoadoutDataMessage1 })]
    public class PlayerLoadoutDataMessage : GameMessage
    {
        public int LoadoutIndex;
        public LoadoutItemData[] Equipment = new LoadoutItemData[13];
        public LoadoutPotionData EqippedPotion;
        public LoadoutSkillData[] ActiveSkills = new LoadoutSkillData[6];
        public int[] /* sno */ SnoTraits = new int[4];
        public int[] /* gbid */ GbidLegendaryPowers = new int[5];
        public string Name;
        public int TabIcon;

        public PlayerLoadoutDataMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            LoadoutIndex = buffer.ReadInt(32);
            for (int i = 0; i < Equipment.Length; i++)
                Equipment[i].Parse(buffer);
            EqippedPotion.Parse(buffer);
            for (int i = 0; i < ActiveSkills.Length; i++)
                ActiveSkills[i].Parse(buffer);
            for (int i = 0; i < SnoTraits.Length; i++)
                SnoTraits[i] = buffer.ReadInt(32);
            for (int i = 0; i < GbidLegendaryPowers.Length; i++)
                GbidLegendaryPowers[i] = buffer.ReadInt(32);
            Name = buffer.ReadCharArray(53);
            TabIcon = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, LoadoutIndex);
            for (int i = 0; i < Equipment.Length; i++)
                Equipment[i].Encode(buffer);
            EqippedPotion.Encode(buffer);
            for (int i = 0; i < ActiveSkills.Length; i++)
                ActiveSkills[i].Encode(buffer);
            for (int i = 0; i < SnoTraits.Length; i++)
                buffer.WriteInt(32, SnoTraits[i]);
            for (int i = 0; i < GbidLegendaryPowers.Length; i++)
                buffer.WriteInt(32, GbidLegendaryPowers[i]);
            buffer.WriteCharArray(53, Name);
            buffer.WriteInt(32, TabIcon);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerHeroStateSkills)]
    public class PlayerSkillsMessage : GameMessage
    {
        public ActiveSkillSavedData[] ActiveSkills;
        public int /* sno */[] Traits;
        public int /* gbid */[] LegendaryPowers;
        public int PlayerIndex;

        public PlayerSkillsMessage()
                    : base(Opcodes.PlayerHeroStateSkills)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActiveSkills = new ActiveSkillSavedData[6];
            for (int i = 0; i < ActiveSkills.Length; i++)
                ActiveSkills[i].Parse(buffer);
            Traits = new int[4];
            for (int i = 0; i < Traits.Length; i++)
                Traits[i] = buffer.ReadInt(32);
            LegendaryPowers = new int[3];
            for (int i = 0; i < LegendaryPowers.Length; i++)
                LegendaryPowers[i] = buffer.ReadInt(32);
            PlayerIndex = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            for (int i = 0; i < ActiveSkills.Length; i++)
                ActiveSkills[i].Encode(buffer);
            for (int i = 0; i < Traits.Length; i++)
                buffer.WriteInt(32, Traits[i]);
            for (int i = 0; i < LegendaryPowers.Length; i++)
                buffer.WriteInt(32, LegendaryPowers[i]);
            buffer.WriteInt(32, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            
        }
    }
}

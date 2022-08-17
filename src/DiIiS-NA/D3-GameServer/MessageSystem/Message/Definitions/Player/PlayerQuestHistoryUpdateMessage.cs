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
    [Message(Opcodes.PlayerQuestHistoryUpdateMessage)]
    public class PlayerQuestHistoryUpdateMessage : GameMessage
    {
        public PlayerQuestHistoryEntry[] Field0;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = new PlayerQuestHistoryEntry[buffer.ReadInt(8)];
            for (int i = 0; i < Field0.Length; i++)
                Field0[i].Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(8, Field0.Length);
            for (int i = 0; i < Field0.Length; i++)
                Field0[i].Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

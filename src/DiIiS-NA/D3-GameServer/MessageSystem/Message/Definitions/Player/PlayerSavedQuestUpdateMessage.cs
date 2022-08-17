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
    [Message(Opcodes.PlayerSavedQuestUpdateMessage)]
    public class PlayerSavedQuestUpdateMessage : GameMessage
    {
        public eField0 Field0;
        public int /* sno */ Field1;
        public int Field2;
        public int Field3;
        public PlayerSavedQuest[] Field4;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = (eField0)buffer.ReadInt(32);
            Field1 = buffer.ReadInt(32);
            Field2 = buffer.ReadInt(32);
            Field3 = buffer.ReadInt(32);
            Field4 = new PlayerSavedQuest[buffer.ReadInt(7)];
            for (int i = 0; i < Field4.Length; i++)
                Field4[i].Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, (int)Field0);
            buffer.WriteInt(32, Field1);
            buffer.WriteInt(32, Field2);
            buffer.WriteInt(32, Field3);
            buffer.WriteInt(7, Field4.Length);
            for (int i = 0; i < Field4.Length; i++)
                Field4[i].Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }

    public enum eField0 : int
    {
        Invalid = 0,
        A1 = 100,
        A2 = 200,
        A3 = 300,
        A4 = 400,
        A5 = 3000,
        OpenWorld = 1000,
        Test = 0
    }
}

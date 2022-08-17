//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields.BlizzLess.Net.GS.Message.Fields;
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
    [Message(Opcodes.HeroStateMessage)]
    public class HeroStateMessage : GameMessage
    {
        public HeroStateData State;
        public int PlayerIndex;
        public HeroStateMessage() : base(Opcodes.HeroStateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            State = new HeroStateData();
            State.Parse(buffer);
            PlayerIndex = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            State.Encode(buffer);
            buffer.WriteInt(32, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            
        }


    }
}

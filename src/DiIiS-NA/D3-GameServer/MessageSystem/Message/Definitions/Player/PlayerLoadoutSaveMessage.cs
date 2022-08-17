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
    [Message(new[] { Opcodes.PlayerLoadoutSaveMessage, Opcodes.PlayerLoadoutSaveMessage1 })]
    public class PlayerLoadoutSaveMessage : GameMessage
    {
        public int Field0;
        public string Name;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Name = buffer.ReadCharArray(53);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            buffer.WriteCharArray(53, Name);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}

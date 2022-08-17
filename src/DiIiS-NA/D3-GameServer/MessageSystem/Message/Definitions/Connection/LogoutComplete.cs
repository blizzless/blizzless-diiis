//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Connection
{
    [Message(Opcodes.LogoutComplete)]
    public class LogoutComplete : GameMessage, ISelfHandler
    {
        public void Handle(GameClient client)
        {
            if (client.IsLoggingOut)
            {
                /*
                client.SendMessage(new QuitGameMessage() // should be sent to all players i guess /raist.
                {
                    PlayerIndex = client.Player.PlayerIndex,
                });
                //*/
                GameManager.RemovePlayerFromGame(client);
            }
        }

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {

        }
    }
}

using DiIiS_NA.GameServer.ClientSystem;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;

namespace DiIiS_NA.GameServer.MessageSystem
{
    public interface ISelfHandler
    {
        void Handle(GameClient client);
    }
}

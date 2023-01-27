using DiIiS_NA.GameServer.ClientSystem;

namespace DiIiS_NA.GameServer.MessageSystem
{
    public interface IMessageConsumer
    {
        void Consume(GameClient client, GameMessage message);
    }

    public enum Consumers
    {
        None,
        ClientManager,
        Game,
        Inventory,
        Conversations,
        Player,
        SelectedNPC
    }
}

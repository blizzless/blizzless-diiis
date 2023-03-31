using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
using DiIiS_NA.GameServer.GSSystem.GameSystem;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.D3_GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
    [HandledSNO(ActorSno._p2_hq_zoltunkulle)] //Zoltun
    //[HandledSNO(431095)] //Wardrobe
    public class ZoltunNPC : Artisan
    {
        public ZoltunNPC(World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            if (world.Game.CurrentAct == ActEnum.OpenWorld)
            {

                Conversations.Add(new ConversationInteraction(430146));
                //[430335] [Worlds] a3dun_ruins_frost_city_a_02
                //[428493] [Worlds] a3dun_ruins_frost_city_a_01
                //this.Attributes[GameAttribute.Conversation_Icon, 0] = 1;

            }
        }

        public override void OnCraft(Player player)
        {
            base.OnCraft(player);
            player.CurrentArtisan = ArtisanType.Cube;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            // TODO: check behavior for campaign mode
            if (World.Game.CurrentAct == ActEnum.OpenWorld && player.KanaiUnlocked)
            {
                // works as ArtisanShortcut if we've found a cube. maybe only after all conversations
                OnCraft(player);
            } else
            {
                base.OnTargeted(player, message);
            }
        }

        public override bool Reveal(Player player)
        {
            if (SNO == ActorSno._kanaicube_stand)
                if (!player.KanaiUnlocked)
                    Interactions.Clear();
                else
                {
                    if (Interactions.Count == 0)
                        Interactions.Add(new CraftInteraction()); 
                }
            return base.Reveal(player);
        }
    }
}

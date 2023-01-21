//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem.Interactions;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
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

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
    [HandledSNO(ActorSno._p2_hq_zoltunkulle)] //Zoltun
    //[HandledSNO(431095)] //Wardrobe
    public class ZoltunNPC : Artisan
    {
        public ZoltunNPC(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            if (world.Game.CurrentAct == 3000)
            {

                this.Conversations.Add(new ConversationInteraction(430146));
                //[430335] [Worlds] a3dun_ruins_frost_city_a_02
                //[428493] [Worlds] a3dun_ruins_frost_city_a_01
                //this.Attributes[GameAttribute.Conversation_Icon, 0] = 1;

            }
        }

        public override void OnTargeted(Player player, MessageSystem.Message.Definitions.World.TargetMessage message)
        {
            base.OnTargeted(player, message);//player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.ANNDataMessage(MessageSystem.Opcodes.OpenArtisanWindowMessage) { ActorID = this.DynamicID });
            player.ArtisanInteraction = "KanaiCube";
        }

        public override bool Reveal(Player player)
        {
            if (this.SNO == ActorSno._kanaicube_stand)
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

    [HandledSNO(ActorSno._kanaicube_stand /* Actor KanaiCube_Stand */)]
    public class CubeShortcut : InteractiveNPC
    {
        public CubeShortcut(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttribute.MinimapActive] = true;
            Attributes[GameAttribute.Conversation_Icon, 0] = 0;
        }

        public override void OnTargeted(Player player, MessageSystem.Message.Definitions.World.TargetMessage message)
        {
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = this.DynamicID(player) });
            player.ArtisanInteraction = "Cube";
        }
    }
}

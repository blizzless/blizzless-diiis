//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
    [HandledSNO(0x0002FA63 /* PT_Blacksmith_ForgeWeaponShortcut.acr */,
        0x0002FA64 /*PT_Blacksmith_ForgeArmorShortcut.acr */,
        0x0002FA62 /*PT_Blacksmith_RepairShortcut.acr */,
        212519 /* Actor PT_Jeweler_AddSocketShortcut */,
        212517 /* Actor PT_Jeweler_CombineShortcut */,
        212521 /* Actor PT_Jeweler_RemoveGemShortcut */,
        212511 /* Actor PT_Mystic_EnhanceShortcut */,
        212510 /* Actor PT_Mystic_IdentifyShortcut */,
        439975 /* KanaiCube_Stand */)]
    public class ArtisanShortcut : InteractiveNPC
    {
        /*
        [437895]p4_Ruins_Frost_KanaiCube_Altar
        [439695] Lore_P3_ZoltunKulle_CubeHistory_01
        [439712] Lore_P3_ZoltunKulle_CubeHistory_02
        [439975] KanaiCube_Stand
        [440510] KanaiCube_Stand_NOCUBE
        [440862] kanai_Cube_EndFlash
        [441103] Kanai_Cube_Standard_FX
        [441219] Kanai_Cube_Standard_FX_LeyLines
        [441225] Kanai_Cube_Standard_FX_MetalWipeIn
        [441276] Kanai_Cube_Standard_FX_Front_Glow
        [441557] p4_Ruins_Frost_KanaiCube_Attract
        [441569] p4_Ruins_Frost_KanaiCube_Altar_Shield
        [441599] TheCubeDiscovery_Kanai_helix
        [441664] p4_Ruins_Frost_KanaiCube_Altar_ClientForUI
        [441932] Kanai_Cube_Standard_FX_Orb
        [441984] Kanai_Cube_Uber_FX_3D_Cube
        [441999] Kanai_Cube_Uber_FX
        [442277] Kanai_Cube_Standard_FX_Front_Glow_Add
        [442282] kanai_Cube_Wash
        [138979] NephalemCube
        //*/
        public ArtisanShortcut(MapSystem.World world, int snoId, TagMap tags)
            : base(world, snoId, tags)
        {
            Attributes[GameAttribute.MinimapActive] = false;
            Attributes[GameAttribute.Conversation_Icon, 0] = 0;
            switch (this.ActorSNO.Id)
            {
                case 0x0002FA62:
                case 0x0002FA63:
                case 0x0002FA64:
                    break;
                case 212517:
                case 212519:
                case 212521:
                    break;
                case 212510:
                case 212511:
                    break;
            }
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = this.DynamicID(player) });
            switch (this.ActorSNO.Id)
            {
                case 0x0002FA62:
                case 0x0002FA63:
                case 0x0002FA64:
                    player.ArtisanInteraction = "Blacksmith";
                    break;
                case 212517:
                case 212519:
                case 212521:
                    player.ArtisanInteraction = "Jeweler";
                    break;
                case 212510:
                case 212511:
                    player.ArtisanInteraction = "Mystic";
                    break;
                case 439975:
                    player.ArtisanInteraction = "Cube";
                    break;
            }
        }
        public override bool Reveal(Player player)
        {
            if(this.World.Game.CurrentAct != 3000)
            switch (this.ActorSNO.Id)
            {
                case 0x0002FA62:
                case 0x0002FA63:
                case 0x0002FA64:
                    if (!player.BlacksmithUnlocked)
                        return false;
                    break;
                case 212517:
                case 212519:
                case 212521:
                    if (!player.JewelerUnlocked)
                        return false;
                    break;
                case 212510:
                case 212511:
                    if (!player.MysticUnlocked)
                        return false;
                    break;
            }

            if (this.ActorSNO.Id == 439975)
                if (!player.KanaiUnlocked)
                    return false;

            return base.Reveal(player);
        }
    }
}

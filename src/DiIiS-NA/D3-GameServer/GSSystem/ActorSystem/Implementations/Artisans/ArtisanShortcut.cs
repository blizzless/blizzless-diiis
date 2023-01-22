﻿//Blizzless Project 2022 
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Artisans
{
    [HandledSNO(
        ActorSno._pt_blacksmith_forgeweaponshortcut /* PT_Blacksmith_ForgeWeaponShortcut.acr */,
        ActorSno._pt_blacksmith_forgearmorshortcut /*PT_Blacksmith_ForgeArmorShortcut.acr */,
        ActorSno._pt_blacksmith_repairshortcut /*PT_Blacksmith_RepairShortcut.acr */,
        ActorSno._pt_jeweler_addsocketshortcut /* Actor PT_Jeweler_AddSocketShortcut */,
        ActorSno._pt_jeweler_combineshortcut /* Actor PT_Jeweler_CombineShortcut */,
        ActorSno._pt_jeweler_removegemshortcut /* Actor PT_Jeweler_RemoveGemShortcut */,
        ActorSno._pt_mystic_enhanceshortcut /* Actor PT_Mystic_EnhanceShortcut */,
        ActorSno._pt_mystic_identifyshortcut /* Actor PT_Mystic_IdentifyShortcut */
    )]
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
        public ArtisanShortcut(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Attributes[GameAttribute.MinimapActive] = false;
            Attributes[GameAttribute.Conversation_Icon, 0] = 0;
        }

        public override void OnTargeted(Player player, TargetMessage message)
        {
            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Misc.ANNDataMessage(Opcodes.OpenArtisanWindowMessage) { ActorID = DynamicID(player) });
            switch (SNO)
            {
                case ActorSno._pt_blacksmith_repairshortcut:
                case ActorSno._pt_blacksmith_forgeweaponshortcut:
                case ActorSno._pt_blacksmith_forgearmorshortcut:
                    player.ArtisanInteraction = "Blacksmith";
                    break;
                case ActorSno._pt_jeweler_combineshortcut:
                case ActorSno._pt_jeweler_addsocketshortcut:
                case ActorSno._pt_jeweler_removegemshortcut:
                    player.ArtisanInteraction = "Jeweler";
                    break;
                case ActorSno._pt_mystic_identifyshortcut:
                case ActorSno._pt_mystic_enhanceshortcut:
                    player.ArtisanInteraction = "Mystic";
                    break;
            }
        }
        public override bool Reveal(Player player)
        {
            if (World.Game.CurrentAct != 3000)
            {
                switch (SNO)
                {
                    case ActorSno._pt_blacksmith_repairshortcut:
                    case ActorSno._pt_blacksmith_forgeweaponshortcut:
                    case ActorSno._pt_blacksmith_forgearmorshortcut:
                        if (!player.BlacksmithUnlocked)
                            return false;
                        break;
                    case ActorSno._pt_jeweler_combineshortcut:
                    case ActorSno._pt_jeweler_addsocketshortcut:
                    case ActorSno._pt_jeweler_removegemshortcut:
                        if (!player.JewelerUnlocked)
                            return false;
                        break;
                    case ActorSno._pt_mystic_identifyshortcut:
                    case ActorSno._pt_mystic_enhanceshortcut:
                        if (!player.MysticUnlocked)
                            return false;
                        break;
                }
            }

            return base.Reveal(player);
        }
    }
}

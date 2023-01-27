using DiIiS_NA.Core.MPQ;
using DiIiS_NA.Core.MPQ.FileFormats;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.AISystem.Brains;
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using MonsterFF = DiIiS_NA.Core.MPQ.FileFormats.Monster;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem
{
    [HandledSNO(ActorSno._tristramgateguardr)]
    class ArrowGuardian : NPC, IUpdateable
    {
        public ArrowGuardian(MapSystem.World world, ActorSno sno, TagMap tags)
            : base(world, sno, tags)
        {
            Brain = new AggressiveNPCBrain(this); // erekose             

            // lookup GameBalance MonsterLevels.gam asset
            var monsterLevels = (GameBalance)MPQStorage.Data.Assets[SNOGroup.GameBalance][19760].Data;
            var monsterData = (Monster.Target as MonsterFF);

            Attributes[GameAttribute.Hitpoints_Max] = 99999;
            Attributes[GameAttribute.Hitpoints_Cur] = Attributes[GameAttribute.Hitpoints_Max_Total];
            Attributes[GameAttribute.Attacks_Per_Second] = 1.0f;
            Attributes[GameAttribute.Damage_Weapon_Min, 0] = 5f;
            Attributes[GameAttribute.Damage_Weapon_Delta, 0] = 5f;
            WalkSpeed = 0f;  // TODO: this is probably multiplied by something erekose the 0.3 is because he is way too fast otherwise

        }

        protected override void ReadTags()
        {
            if (!Tags.ContainsKey(MarkerKeys.ConversationList))
            {
                Tags.Add(MarkerKeys.ConversationList, new TagMapEntry(MarkerKeys.ConversationList.ID, 108832, 2));
            }

            base.ReadTags();
        }
        public override bool Reveal(Player player)
        {
            if (!base.Reveal(player))
                return false;

            Attributes[GameAttribute.TeamID] = 2;
            return true;
        }

        public void Update(int tickCounter)
        {
            if (Brain == null)
                return;
        }
    }
}

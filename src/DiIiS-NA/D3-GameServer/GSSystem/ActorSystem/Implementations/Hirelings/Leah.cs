using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.ActorSystem.Implementations.Hirelings
{
    // TODO: Check for copy-paste from Scoundrel
    //[HandledSNO(144681 /* Leah_Party.acr */)]
    public class Leah : Hireling
	{
		public Leah(World world, ActorSno sno, TagMap tags)
			: base(world, sno, tags)
		{
			mainSNO = ActorSno._leah;
			hirelingSNO = ActorSno._hireling_scoundrel;
			proxySNO = ActorSno._hireling_scoundrel_proxy;
			skillKit = 0x8AFE;
			hirelingGBID = StringHashHelper.HashItemName("Scoundrel");
			Attributes[GameAttribute.Hireling_Class] = 4;
			var MS = Attributes[GameAttribute.Movement_Scalar];
			var RS = Attributes[GameAttribute.Run_Speed_Granted];
			var MSRP = Attributes[GameAttribute.Movement_Scalar_Reduction_Percent];
			Attributes[GameAttribute.Movement_Scalar] = 3f;
			Attributes[GameAttribute.Run_Speed_Granted] = 3f;
			//this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			WalkSpeed = 0.3f; Attributes[GameAttribute.Hitpoints_Max] = 9999f;
			var HPM = Attributes[GameAttribute.Hitpoints_Max];
			var HPMT = Attributes[GameAttribute.Hitpoints_Max_Total];

			Attributes[GameAttribute.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
            
            Attributes[GameAttribute.Hitpoints_Max] = Attributes[GameAttribute.Hitpoints_Max_Total];

		}

		public override Hireling CreateHireling(World world, ActorSno sno, TagMap tags)
		{
			return new Leah(world, sno, tags);
		}

		public override bool Reveal(Player player)
		{
			if (!base.Reveal(player))
				return false;

            player.InGameClient.SendMessage(new MessageSystem.Message.Definitions.Inventory.VisualInventoryMessage()
            {
                ActorID = DynamicID(player),
                EquipmentList = new MessageSystem.Message.Fields.VisualEquipment()
                {
                    Equipment = new MessageSystem.Message.Fields.VisualItem[]
                    {
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                       new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = unchecked((int)0xACA5382),
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,//0x6C3B0389,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                        new MessageSystem.Message.Fields.VisualItem()
                        {
                            GbId = -1,
                            DyeType = 0,
                            ItemEffectType = 0,
                            EffectLevel = -1,
                        },
                    }
                }
            });
            Attributes[GameAttribute.Conversation_Icon, 0] = 1;
            Attributes.BroadcastChangedIfRevealed();

            return true;
		}
	}
}

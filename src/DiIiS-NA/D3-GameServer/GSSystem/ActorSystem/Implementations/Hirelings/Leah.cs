using DiIiS_NA.Core.Helpers.Hash;
using DiIiS_NA.D3_GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.MapSystem;
using DiIiS_NA.GameServer.GSSystem.PlayerSystem;
using DiIiS_NA.GameServer.MessageSystem;

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
			Attributes[GameAttributes.Hireling_Class] = 4;
			var MS = Attributes[GameAttributes.Movement_Scalar];
			var RS = Attributes[GameAttributes.Run_Speed_Granted];
			var MSRP = Attributes[GameAttributes.Movement_Scalar_Reduction_Percent];
			Attributes[GameAttributes.Movement_Scalar] = 3f;
			Attributes[GameAttributes.Run_Speed_Granted] = 3f;
			//this.Attributes[GameAttribute.Movement_Scalar_Reduction_Percent] -= 20f;
			WalkSpeed = 0.3f; Attributes[GameAttributes.Hitpoints_Max] = 9999f;
			var HPM = Attributes[GameAttributes.Hitpoints_Max];
			var HPMT = Attributes[GameAttributes.Hitpoints_Max_Total];

			Attributes[GameAttributes.Hitpoints_Max_Percent_Bonus_Multiplicative] = 1;
            
            Attributes[GameAttributes.Hitpoints_Max] = Attributes[GameAttributes.Hitpoints_Max_Total];

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
            Attributes[GameAttributes.Conversation_Icon, 0] = 1;
            Attributes.BroadcastChangedIfRevealed();

            return true;
		}
	}
}

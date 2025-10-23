using System.Text;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(new[] {
          Opcodes.OpenArtisanWindowMessage,   //Opcodes.OpenTradeWindow,
        //Opcodes.RequestBuyItemMessage,  Opcodes.RequestSellItemMessage,
        Opcodes.TrickleRemove
        , Opcodes.InventoryStackNotifyMessage,  Opcodes.InventoryStackDecreaseNotifyMessage//,  Opcodes.ANNDataMessage10
        , Opcodes.InventoryDyeNotifyMessage, Opcodes.InventorySwapToBody, Opcodes.TryPortraitDropMessage
        , Opcodes.PlayIdleAnimationMessage, Opcodes.OpenWaypointSelectionWindowMessage, Opcodes.NPCTalkRequestMessage, Opcodes.NPCVendorRequestMessage//, Opcodes.ANNDataMessage20
        , Opcodes.InventoryCreateMessage, Opcodes.ShrineActivatedMessage, Opcodes.ActivatePoolOfReflectionMessage, Opcodes.ResetDeathFadeTimeMessage, Opcodes.RemoveFromSphereList
        , Opcodes.CancelACDTargetMessage, Opcodes.ResetACDEffectsMessage, Opcodes.ObjectiveFinderActivated, Opcodes.RequestDebugActorTooltipMessage, Opcodes.RiftStartEncounterMessage
        , Opcodes.ReforgeMessage, Opcodes.ReforgeResultsMessage, Opcodes.ClientChooseEnchantAffix , Opcodes.RepairMessage, Opcodes.MysteyItemResultsMessage
        , Opcodes.ShrineGlobeTextDisplay, Opcodes.RequestPromoteToPartyGuide
        , Opcodes.ANNDataMessage43, Opcodes.ANNDataMessage44, Opcodes.ANNDataMessage45, Opcodes.ANNDataMessage46, Opcodes.OpenSharedStashMessage
    })]
    public class ANNDataMessage : GameMessage
    {
        public uint ActorID; // Actor's DynamicID

        public ANNDataMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ANNDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Base
{
    [Message(
      new[] {
          Opcodes.LeaveConsoleGame,
          Opcodes.DeathAckMessage,
          Opcodes.CombatLogToogleMessage,
          Opcodes.GameSetupMessageDone,
          Opcodes.ItemDeleteMessage,
          Opcodes.StopMusicOverlayMessage,
          Opcodes.OpenBannerCustomizationMessage,
          Opcodes.AcceptResurrectMessage,
          Opcodes.OpenCosmeticsMessage,
          Opcodes.ForceNPCInteractClose,
          Opcodes.InspectEndMessage,
          Opcodes.TriggerLightingMessage,
          Opcodes.Ping,
          Opcodes.KillCounterRefresh,
          Opcodes.DemoOverMessagge,
          Opcodes.BetaOverMessage,
          Opcodes.PVPArenaRestartMessage,
          Opcodes.LoadLegendaryPowerIconsMessage,
          Opcodes.ResetExtractedLegendaries,
          Opcodes.BossStartDeclinedMessage,
          Opcodes.BossJoinDeclinedMessage,
          Opcodes.BossEncounterStartedMessage,
          Opcodes.BossEncounterCanceledMessagge,
          Opcodes.BossVoteContinueMessage,
          Opcodes.BossVoteCancelMessagge,
          Opcodes.SetDungeonCanceledMessage,
          Opcodes.SetDungeonStartedMessage,
          Opcodes.SetDungeonJoinDeclinedMessage,
          Opcodes.SetDungeonJoinAcceptedMessage,
          Opcodes.SetDungeonVoteContinueMessage,
          Opcodes.SetDungeonVoteCancelMessage,
          Opcodes.RiftStartDeclinedMessageg,
          Opcodes.RiftJoinDeclinedMessage,
          Opcodes.RiftJoinInfoRequest,
          Opcodes.RiftStartedMessage,
          Opcodes.RiftCanceledMessage,
          Opcodes.RiftVoteCancelMessage,
          Opcodes.CameraSriptedSequenceStopMessage,
          Opcodes.CameraTriggerFadeMessage,
          Opcodes.CameraTriggerInstantFateToBlack,
          Opcodes.RepairAckMessage,
          Opcodes.UseIdentifyAllItemMessagge,
          Opcodes.CloseGameMessage,
          Opcodes.GameTestingStopSampling,
          Opcodes.NPCInteractCancel,
          Opcodes.TriedSalvage,
          Opcodes.PoolMailMessage,
          Opcodes.FetchMailMessage,
          Opcodes.RedeemPVPTokens,
          Opcodes.GameEndedMessage,
          Opcodes.ClearFanfareQueue,
          Opcodes.MultiplayerBuffOn,
          Opcodes.MultiplayerBuffOff,
          Opcodes.SetDungeonAcceptMessage,
          Opcodes.SetDungeonCannotStartMessage,
          Opcodes.DynamicQuestResetMessage,
          Opcodes.DungeonFinderBossStart,
          Opcodes.NewSkillKitMessage,
          Opcodes.OpenWorldModeChangeAcceptMessage,
          Opcodes.OpenWorldModeResetRequestMessage,
          Opcodes.OpenWorldTutorial,
          Opcodes.DifficultyLoweredSavePrefs,
          Opcodes.DifficultyLoweredFlag,
          Opcodes.DifficultyRaisedSavePrefs,
          Opcodes.DifficultyRaisedFlagg,
          Opcodes.SimpleMessage91,
          Opcodes.SimpleMessage92,
          Opcodes.SimpleMessage93,
          Opcodes.SimpleMessage94,
          Opcodes.SimpleMessage95
    })]
    public class SimpleMessage : GameMessage
    {
        public SimpleMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("SimpleMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

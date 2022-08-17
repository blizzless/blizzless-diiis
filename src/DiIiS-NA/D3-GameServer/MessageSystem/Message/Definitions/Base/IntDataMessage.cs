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
    [Message(new[] {
        //Opcodes.IntDataMessage,
        Opcodes.AutoAssignedToButtonMessage,
        Opcodes.DisplayErrorMessage,
        Opcodes.SetScreenshotViewMessage,
        Opcodes.BossEncounterOneDeclinedMessage,
        Opcodes.SetDungeonOneDeclinedMessage,
        //Opcodes.IntDataMessage6,
        Opcodes.RiftOneDeclinedMessage,
        Opcodes.ServerChooseEnchantAffix,
        Opcodes.TransumteFailedMessage,
        Opcodes.ResultsTrainArtisanMessage,
        Opcodes.ActTransitionRequestOpenWorldMessage,
        Opcodes.ConsoleSyncHeroSavedConversations,
        Opcodes.SaveBannerResponseMessage,
        //Opcodes.IntDataMessage14,
        Opcodes.ParagonPurchaseResetCategory,
        Opcodes.SetDungeonClosingMessage,
        Opcodes.DungeonFinderSeedMessage,
        Opcodes.DungeonFinderParticipatingPlayerCount,
        Opcodes.LowedGameDifficulty,
        Opcodes.RaisedGameDifficulty,
        Opcodes.IntDataMessage21,
        Opcodes.IntDataMessage22,
        Opcodes.IntDataMessage23,
        Opcodes.IntDataMessage24,
        Opcodes.IntDataMessage25,
        Opcodes.IntDataMessage26,
        })]
    public class IntDataMessage : GameMessage
    {
        public int Field0;
        public IntDataMessage(Opcodes opcode) : base(opcode) { }
        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("IntDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

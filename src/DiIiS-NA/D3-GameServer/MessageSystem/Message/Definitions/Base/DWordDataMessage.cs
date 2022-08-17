//Blizzless Project 2022 
using DiIiS_NA.GameServer.ClientSystem;
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
        Opcodes.RequestLANGameParamsMessage, Opcodes.CancelLoopingSkillMessage,
        Opcodes.CancelWalkMessageg,Opcodes.DisplayVagueLockedErrorMessage,
        Opcodes.TrialOverMessagge,Opcodes.SendPlayerPrefsMessage, Opcodes.AccountFlagsSyncMessage,Opcodes.AccountStashTabsRewardedForSeasonSyncMessagge,Opcodes.ConsoleSaveMessage,
        Opcodes.ConsoleSyncHeroActiveHireling,Opcodes.TeleportToWaypoint,Opcodes.DWordDataMessage16,Opcodes.DWordDataMessage17,Opcodes.DWordDataMessage18})]
    public class DWordDataMessage : GameMessage, ISelfHandler
    {
        public int Field0;

        public DWordDataMessage() { }
        public DWordDataMessage(int opcode) : base(opcode) { }

        public void Handle(GameClient client)
        {

        }

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
            b.AppendLine("DWordDataMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + Field0.ToString("X8") + " (" + Field0 + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

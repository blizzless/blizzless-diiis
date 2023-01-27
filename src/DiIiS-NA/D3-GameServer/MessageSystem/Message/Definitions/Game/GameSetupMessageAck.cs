using DiIiS_NA.Core.Logging;
using DiIiS_NA.GameServer.ClientSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.GameSetupMessageFinished)]
    public class GameSetupMessageAck : GameMessage, ISelfHandler
    {
        public int FirstHeartBeat;

        //private static readonly Logger Logger = LogManager.CreateLogger();

        public GameSetupMessageAck() : base(Opcodes.GameSetupMessageFinished) { }

        public override void Parse(GameBitBuffer buffer)
        {
            //FirstHeartBeat = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            //buffer.WriteInt(32, FirstHeartBeat);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("GameSetupMessageAck:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("FirstHeartBeat: 0x" + FirstHeartBeat.ToString("X8") + " (" + FirstHeartBeat + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public void Handle(GameClient client)
        {
            Logger.Info("Client {0}, enter in game. Server loaded.", client.BnetClient.Account.BattleTag);
        }
    }
}

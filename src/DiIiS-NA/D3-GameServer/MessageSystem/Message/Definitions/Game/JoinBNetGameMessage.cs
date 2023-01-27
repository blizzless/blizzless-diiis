using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.JoinBNetGameMessage, Consumers.ClientManager)]
    public class JoinBNetGameMessage : GameMessage
    {
        public ulong HeroID;
        public int SGameId; 
        public ulong AuthToken;
        public int XLocale; 
        public int ProtocolHash;
        public int SNOPackHash;

        public override void Parse(GameBitBuffer buffer)
        {
            HeroID = buffer.ReadUInt64(64);
            SGameId = buffer.ReadInt(32);
            AuthToken = buffer.ReadUInt64(64);
            XLocale = buffer.ReadInt(5) + (2);
            ProtocolHash = buffer.ReadInt(32);
            SNOPackHash = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt64(64, HeroID);
            buffer.WriteInt(32, SGameId);
            buffer.WriteUInt64(64, AuthToken);
            buffer.WriteInt(5, XLocale - (2));
            buffer.WriteInt(32, ProtocolHash);
            buffer.WriteInt(32, SNOPackHash);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("JoinBNetGameMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HeroID: 0x" + HeroID.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("SGameId: 0x" + SGameId.ToString("X8") + " (" + SGameId + ")");
            b.Append(' ', pad); b.AppendLine("AuthToken: 0x" + AuthToken.ToString("X16"));
            b.Append(' ', pad); b.AppendLine("XLocale: 0x" + XLocale.ToString("X8") + " (" + XLocale + ")");
            b.Append(' ', pad); b.AppendLine("ProtocolHash: 0x" + ProtocolHash.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("SNOPackHash: 0x" + SNOPackHash.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

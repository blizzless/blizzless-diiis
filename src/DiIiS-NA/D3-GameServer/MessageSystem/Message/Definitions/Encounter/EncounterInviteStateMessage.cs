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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Encounter
{
    [Message(Opcodes.EncounterInviteStateMessage)]
    public class EncounterInviteStateMessage : GameMessage
    {
        public byte AcceptedPlayers;
        public byte PendingPlayers;
        public byte DeclinedPlayers;

        public override void Parse(GameBitBuffer buffer)
        {
            AcceptedPlayers = (byte)buffer.ReadInt(8);
            PendingPlayers = (byte)buffer.ReadInt(8);
            DeclinedPlayers = (byte)buffer.ReadInt(8);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(8, AcceptedPlayers);
            buffer.WriteInt(8, PendingPlayers);
            buffer.WriteInt(8, DeclinedPlayers);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EncounterInviteStateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("AcceptedPlayers: 0x" + AcceptedPlayers.ToString("X8") + " (" + AcceptedPlayers + ")");
            b.Append(' ', pad); b.AppendLine("PendingPlayers: 0x" + PendingPlayers.ToString("X8") + " (" + PendingPlayers + ")");
            b.Append(' ', pad); b.AppendLine("DeclinedPlayers: 0x" + DeclinedPlayers.ToString("X8") + " (" + DeclinedPlayers + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

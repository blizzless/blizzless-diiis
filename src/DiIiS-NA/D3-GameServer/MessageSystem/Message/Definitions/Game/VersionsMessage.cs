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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Game
{
    [Message(Opcodes.VersionsMessage)]
    public class VersionsMessage : GameMessage
    {
        public uint SNOPackHash;
        public uint ProtocolHash;
        public string Version;

        public VersionsMessage(uint snoPacketHash, uint protocol = 0xCB32584D) : base(Opcodes.VersionsMessage)
        {
            this.SNOPackHash = snoPacketHash;
            this.ProtocolHash = protocol;
            this.Version = "DiIiS Server - 2.7.4.84161";
        }

        public VersionsMessage() : base(Opcodes.VersionsMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            SNOPackHash = buffer.ReadUInt(32);
            ProtocolHash = buffer.ReadUInt(32);
            Version = buffer.ReadCharArray(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, SNOPackHash);
            buffer.WriteUInt(32, ProtocolHash);
            buffer.WriteCharArray(32, Version);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("VersionsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("SNOPackHash: 0x" + SNOPackHash.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("ProtocolHash: 0x" + ProtocolHash.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Version: \"" + Version + "\"");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

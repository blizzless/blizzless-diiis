//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Conversation
{
    [Message(Opcodes.PlayConvLineMessage)]
    public class PlayConvLineMessage : GameMessage
    {
        public uint ActorID;      
        // MaxLength = 9
        public uint[] Field1;     
        public PlayLineParams PlayLineParams;
        public int Duration;      

        public PlayConvLineMessage() : base(Opcodes.PlayConvLineMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            Field1 = new uint[9];
            for (int i = 0; i < Field1.Length; i++) Field1[i] = buffer.ReadUInt(32);
            PlayLineParams = new PlayLineParams();
            PlayLineParams.Parse(buffer);
            Duration = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            for (int i = 0; i < Field1.Length; i++) buffer.WriteUInt(32, Field1[i]);
            PlayLineParams.Encode(buffer);
            buffer.WriteInt(32, Duration);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayConvLineMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("Field1:");
            b.Append(' ', pad); b.AppendLine("{");
            for (int i = 0; i < Field1.Length;) { b.Append(' ', pad + 1); for (int j = 0; j < 8 && i < Field1.Length; j++, i++) { b.Append("0x" + Field1[i].ToString("X8") + ", "); } b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            PlayLineParams.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("Duration: 0x" + Duration.ToString("X8") + " (" + Duration + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

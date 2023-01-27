using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class ActiveEvent
    {
        public int /* sno */ snoTimedEvent;
        public int StartTime;
        public int ExpirationTime;
        public int ArtificiallyElapsedTime;

        public void Parse(GameBitBuffer buffer)
        {
            snoTimedEvent = buffer.ReadInt(32);
            StartTime = buffer.ReadInt(32);
            ExpirationTime = buffer.ReadInt(32);
            ArtificiallyElapsedTime = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoTimedEvent);
            buffer.WriteInt(32, StartTime);
            buffer.WriteInt(32, ExpirationTime);
            buffer.WriteInt(32, ArtificiallyElapsedTime);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ActiveEvent:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("snoTimedEvent: 0x" + snoTimedEvent.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("StartTime: 0x" + StartTime.ToString("X8") + " (" + StartTime + ")");
            b.Append(' ', pad);
            b.AppendLine("ExpirationTime: 0x" + ExpirationTime.ToString("X8") + " (" + ExpirationTime + ")");
            b.Append(' ', pad);
            b.AppendLine("ArtificiallyElapsedTime: 0x" + ArtificiallyElapsedTime.ToString("X8") + " (" + ArtificiallyElapsedTime + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

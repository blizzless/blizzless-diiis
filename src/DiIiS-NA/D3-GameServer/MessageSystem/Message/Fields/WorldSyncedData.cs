using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public class WorldSyncedData
    {
        public int SnoWeatherOverride;
        public float WeatherIntensityOverride;
        public int WeatherIntensityOverrideEnd;

        public void Parse(GameBitBuffer buffer)
        {
            SnoWeatherOverride = buffer.ReadInt(32);
            WeatherIntensityOverride = buffer.ReadFloat32();
            WeatherIntensityOverrideEnd = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SnoWeatherOverride);
            buffer.WriteFloat32(WeatherIntensityOverride);
            buffer.WriteInt(32, WeatherIntensityOverrideEnd);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldLocationMessageData:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("SnoWeatherOverride: 0x" + SnoWeatherOverride.ToString("X8") + " (" + SnoWeatherOverride + ")");
            b.Append(' ', pad);
            b.AppendLine("WeatherIntensityOverride: 0x" + WeatherIntensityOverride.ToString("F") + " (" + WeatherIntensityOverride + ")");
            b.Append(' ', pad);
            b.AppendLine("WeatherIntensityOverrideEnd: 0x" + WeatherIntensityOverrideEnd.ToString("X8") + " (" + WeatherIntensityOverrideEnd + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

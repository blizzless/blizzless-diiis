using DiIiS_NA.Core.MPQ.FileFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiIiS_NA.Core.MPQ.FileFormats.GameBalance;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Fields
{
    public enum VoiceGender
    {
        Male = 0,
        Female = 1,
    }


    public class PlayLineParams
    {
        public int SNOConversation;
        public int SpeakingPlayerIndex; 
        public bool Zoom;        
        public bool FirstLine;
        public bool Hostiile;
        public bool PlayerInitiated;

        public int LineID;
        public Speaker Speaker;
        public int LineGender;         

        public Class TextClass;

        public VoiceGender Gender;

        public Class AudioClass;

        public int SNOSpeakerActor;

        public int LineFlags;

        public int AnimationTag;

        public int Duration;

        public int Id;
        public int Priority;

        public void Parse(GameBitBuffer buffer)
        {
            SNOConversation = buffer.ReadInt(32);
            SpeakingPlayerIndex = buffer.ReadInt(32);
            Zoom = buffer.ReadBool();
            FirstLine = buffer.ReadBool();
            Hostiile = buffer.ReadBool();
            PlayerInitiated = buffer.ReadBool();
            LineID = buffer.ReadInt(32);
            Speaker = (Speaker)buffer.ReadInt(32);
            LineGender = buffer.ReadInt(32);
            TextClass = (Class)buffer.ReadInt(32);
            Gender = (VoiceGender)buffer.ReadInt(32);
            AudioClass = (Class)buffer.ReadInt(32);
            SNOSpeakerActor = buffer.ReadInt(32);
            LineFlags = buffer.ReadInt(32);
            AnimationTag = buffer.ReadInt(32);
            Duration = buffer.ReadInt(32);
            Id = buffer.ReadInt(32);
            Priority = buffer.ReadInt(32);
        }

        public void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, SNOConversation);
            buffer.WriteInt(32, SpeakingPlayerIndex);
            buffer.WriteBool(Zoom);
            buffer.WriteBool(FirstLine);
            buffer.WriteBool(Hostiile);
            buffer.WriteBool(PlayerInitiated);
            buffer.WriteInt(32, LineID);
            buffer.WriteInt(32, (int)Speaker);
            buffer.WriteInt(32, LineGender);
            buffer.WriteInt(32, (int)TextClass);
            buffer.WriteInt(32, (int)Gender);
            buffer.WriteInt(32, (int)AudioClass);
            buffer.WriteInt(32, SNOSpeakerActor);
            buffer.WriteInt(32, LineFlags);
            buffer.WriteInt(32, AnimationTag);
            buffer.WriteInt(32, Duration);
            buffer.WriteInt(32, Id);
            buffer.WriteInt(32, Priority);
        }

        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayLineParams:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("snoConversation: 0x" + SNOConversation.ToString("X8"));
            b.Append(' ', pad);
            b.AppendLine("SpeakingPlayerIndex: 0x" + SpeakingPlayerIndex.ToString("X8") + " (" + SpeakingPlayerIndex + ")");
            b.Append(' ', pad);
            b.AppendLine("Zoom: " + (Zoom ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("FirstLine: " + (FirstLine ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("Hostiile: " + (Hostiile ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("PlayerInitiated: " + (PlayerInitiated ? "true" : "false"));
            b.Append(' ', pad);
            b.AppendLine("LineID: 0x" + LineID.ToString("X8") + " (" + LineID + ")");
            b.Append(' ', pad);
            b.AppendLine("Speaker: 0x" + ((int)Speaker).ToString("X8") + " (" + Speaker + ")");
            b.Append(' ', pad);
            b.AppendLine("LineGender: 0x" + LineGender.ToString("X8") + " (" + LineGender + ")");
            b.Append(' ', pad);
            b.AppendLine("TextClass: 0x" + ((int)TextClass).ToString("X8") + " (" + TextClass + ")");
            b.Append(' ', pad);
            b.AppendLine("Flags: 0x" + ((int)Gender).ToString("X8") + " (" + Gender + ")");
            b.Append(' ', pad);
            b.AppendLine("AudioClass: 0x" + ((int)AudioClass).ToString("X8") + " (" + AudioClass + ")");
            b.Append(' ', pad);
            b.AppendLine("snoSpeakerActor: 0x" + SNOSpeakerActor.ToString("X8"));
            
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

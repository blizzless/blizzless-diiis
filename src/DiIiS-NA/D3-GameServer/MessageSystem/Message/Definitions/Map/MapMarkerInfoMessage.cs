using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Map
{
    [Message(Opcodes.MapMarkerInfoMessage)]
    public class MapMarkerInfoMessage : GameMessage
    {
        public int HashedName;
        public WorldPlace Place;
        public int ImageInfo;
        public int /* sno */ snoStringList;
        public int /* sno */ snoKnownActorOverride;
        public int /* sno */ snoQuestSource;
        public int Label;
        public float MaxDislpayRangeSq;
        public float MinDislpayRangeSq;
        public float DiscoveryRangeSq;
        public int Image;
        public bool Active;
        public bool CanBecomeArrow;
        public bool RespectsFoW;
        public bool IsPing;
        public int PlayerUseFlags;

        public MapMarkerInfoMessage()
            : base(Opcodes.MapMarkerInfoMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            HashedName = buffer.ReadInt(32);
            Place = new WorldPlace();
            Place.Parse(buffer);
            ImageInfo = buffer.ReadInt(32);
            snoStringList = buffer.ReadInt(32);
            snoKnownActorOverride = buffer.ReadInt(32);
            snoQuestSource = buffer.ReadInt(32);
            Label = buffer.ReadInt(32);
            MaxDislpayRangeSq = buffer.ReadFloat32();
            MinDislpayRangeSq = buffer.ReadFloat32();
            DiscoveryRangeSq = buffer.ReadFloat32();
            Image = buffer.ReadInt(32);
            Active = buffer.ReadBool();
            CanBecomeArrow = buffer.ReadBool();
            RespectsFoW = buffer.ReadBool();
            IsPing = buffer.ReadBool();
            PlayerUseFlags = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, HashedName);
            Place.Encode(buffer);
            buffer.WriteInt(32, ImageInfo);
            buffer.WriteInt(32, snoStringList);
            buffer.WriteInt(32, snoKnownActorOverride);
            buffer.WriteInt(32, snoQuestSource);
            buffer.WriteInt(32, Label);
            buffer.WriteFloat32(MaxDislpayRangeSq);
            buffer.WriteFloat32(MinDislpayRangeSq);
            buffer.WriteFloat32(DiscoveryRangeSq);
            buffer.WriteInt(32,Image);
            buffer.WriteBool(Active);
            buffer.WriteBool(CanBecomeArrow);
            buffer.WriteBool(RespectsFoW);
            buffer.WriteBool(IsPing);
            buffer.WriteInt(32, PlayerUseFlags);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("MapMarkerInfoMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("HashedName: 0x" + HashedName.ToString("X8") + " (" + HashedName + ")");
            Place.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("ImageInfo: 0x" + ImageInfo.ToString("X8") + " (" + ImageInfo + ")");
            b.Append(' ', pad); b.AppendLine("snoStringList: 0x" + snoStringList.ToString("X8") + " (" + snoStringList + ")");
            b.Append(' ', pad); b.AppendLine("snoKnownActorOverride: 0x" + snoKnownActorOverride.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("snoQuestSource: 0x" + snoQuestSource.ToString("X8") + " (" + snoQuestSource + ")");
            b.Append(' ', pad); b.AppendLine("Label: 0x" + Label.ToString("X8") + " (" + Label + ")");
            b.Append(' ', pad); b.AppendLine("MaxDislpayRangeSq: " + MaxDislpayRangeSq.ToString("G"));
            b.Append(' ', pad); b.AppendLine("MinDislpayRangeSq: " + MinDislpayRangeSq.ToString("G"));
            b.Append(' ', pad); b.AppendLine("DiscoveryRangeSq: " + DiscoveryRangeSq.ToString("G"));
            b.Append(' ', pad); b.AppendLine("Image: 0x" + Image.ToString("X8") + " (" + Image + ")");
            b.Append(' ', pad); b.AppendLine("Active: " + (Active ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("CanBecomeArrow: " + (CanBecomeArrow ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("RespectsFoW: " + (RespectsFoW ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("IsPing: " + (IsPing ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("PlayerUseFlags: 0x" + PlayerUseFlags.ToString("X8") + " (" + PlayerUseFlags + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

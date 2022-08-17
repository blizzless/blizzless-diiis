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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Misc
{
    [Message(Opcodes.TrickleMessage)]
    public class TrickleMessage : GameMessage
    {
        public uint ActorId;
        public int ActorSNO;
        public WorldPlace WorldLocation;
        public int? PlayerIndex;    
        public int LevelAreaSNO;    
        public float? HealthPercent;
        public int TrickleInfoType;
        public int TrickleFlags;
        public int? HeadstoneCorpseReviveTime;            
        public int? MinimapTextureSNO;  
        public int? MinimapStringLabel;            
        public int? MinimapIconLabel;            
        public int? StringListSNO;  
        public float? MinDisplayRangeSq;      
        public float? MaxDisplayRangeSq;      
        public int? Action;
        public int? EngagedWithRareTime;
        public int? EngagedWithGoblinTime;
        public bool? InCombat;
        public int? TieredRiftRank;
        public bool? Brawling;
        public bool? InUberFight;
        public int? gbidPortraitFrame;
        public bool? field23;

        public TrickleMessage() : base(Opcodes.TrickleMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            ActorSNO = buffer.ReadInt(32);
            WorldLocation = new WorldPlace();
            WorldLocation.Parse(buffer);
            if (buffer.ReadBool())
            {
                PlayerIndex = buffer.ReadInt(4) + (-1);
            }
            LevelAreaSNO = buffer.ReadInt(32);
            if (buffer.ReadBool())
            {
                HealthPercent = buffer.ReadFloat32();
            }
            TrickleInfoType = buffer.ReadInt(4);
            TrickleFlags = buffer.ReadInt(10);
            if (buffer.ReadBool())
            {
                HeadstoneCorpseReviveTime = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MinimapTextureSNO = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MinimapStringLabel = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MinimapIconLabel = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                StringListSNO = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MinDisplayRangeSq = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                MaxDisplayRangeSq = buffer.ReadFloat32();
            }
            if (buffer.ReadBool())
            {
                Action = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                EngagedWithRareTime = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                EngagedWithGoblinTime = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                InCombat = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                TieredRiftRank = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                Brawling = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                InUberFight = buffer.ReadBool();
            }
            if (buffer.ReadBool())
            {
                gbidPortraitFrame = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                field23 = buffer.ReadBool();
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            buffer.WriteInt(32, ActorSNO);
            WorldLocation.Encode(buffer);
            buffer.WriteBool(PlayerIndex.HasValue);
            if (PlayerIndex.HasValue)
            {
                buffer.WriteInt(4, PlayerIndex.Value - (-1));
            }
            buffer.WriteInt(32, LevelAreaSNO);
            buffer.WriteBool(HealthPercent.HasValue);
            if (HealthPercent.HasValue)
            {
                buffer.WriteFloat32(HealthPercent.Value);
            }
            buffer.WriteInt(4, TrickleInfoType);
            buffer.WriteInt(10, TrickleFlags);
            buffer.WriteBool(HeadstoneCorpseReviveTime.HasValue);
            if (HeadstoneCorpseReviveTime.HasValue)
            {
                buffer.WriteInt(32, HeadstoneCorpseReviveTime.Value);
            }
            buffer.WriteBool(MinimapTextureSNO.HasValue);
            if (MinimapTextureSNO.HasValue)
            {
                buffer.WriteInt(32, MinimapTextureSNO.Value);
            }
            buffer.WriteBool(MinimapStringLabel.HasValue);
            if (MinimapStringLabel.HasValue)
            {
                buffer.WriteInt(32, MinimapStringLabel.Value);
            }
            buffer.WriteBool(MinimapIconLabel.HasValue);
            if (MinimapIconLabel.HasValue)
            {
                buffer.WriteInt(32, MinimapIconLabel.Value);
            }
            buffer.WriteBool(StringListSNO.HasValue);
            if (StringListSNO.HasValue)
            {
                buffer.WriteInt(32, StringListSNO.Value);
            }
            buffer.WriteBool(MinDisplayRangeSq.HasValue);
            if (MinDisplayRangeSq.HasValue)
            {
                buffer.WriteFloat32(MinDisplayRangeSq.Value);
            }
            buffer.WriteBool(MaxDisplayRangeSq.HasValue);
            if (MaxDisplayRangeSq.HasValue)
            {
                buffer.WriteFloat32(MaxDisplayRangeSq.Value);
            }
            buffer.WriteBool(Action.HasValue);
            if (Action.HasValue)
            {
                buffer.WriteInt(32, EngagedWithGoblinTime.Value);
            }
            buffer.WriteBool(EngagedWithRareTime.HasValue);
            if (EngagedWithRareTime.HasValue)
            {
                buffer.WriteInt(32, EngagedWithGoblinTime.Value);
            }
            buffer.WriteBool(EngagedWithGoblinTime.HasValue);
            if (EngagedWithGoblinTime.HasValue)
            {
                buffer.WriteInt(32, EngagedWithGoblinTime.Value);
            }
            buffer.WriteBool(InCombat.HasValue);
            if (InCombat.HasValue)
            {
                buffer.WriteBool(InCombat.Value);
            }
            buffer.WriteBool(TieredRiftRank.HasValue);
            if (TieredRiftRank.HasValue)
            {
                buffer.WriteInt(32, TieredRiftRank.Value);
            }
            buffer.WriteBool(Brawling.HasValue);
            if (Brawling.HasValue)
            {
                buffer.WriteBool(Brawling.Value);
            }
            buffer.WriteBool(InUberFight.HasValue);
            if (InUberFight.HasValue)
            {
                buffer.WriteBool(InUberFight.Value);
            }
            buffer.WriteBool(gbidPortraitFrame.HasValue);
            if (gbidPortraitFrame.HasValue)
            {
                buffer.WriteInt(32, gbidPortraitFrame.Value);
            }
            buffer.WriteBool(field23.HasValue);
            if (InUberFight.HasValue)
            {
                buffer.WriteBool(field23.Value);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TrickleMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', pad); b.AppendLine("ActorSNO: 0x" + ActorSNO.ToString("X8"));
            WorldLocation.AsText(b, pad);
            if (PlayerIndex.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("PlayerIndex.Value: 0x" + PlayerIndex.Value.ToString("X8") + " (" + PlayerIndex.Value + ")");
            }
            b.Append(' ', pad); b.AppendLine("LevelAreaSNO: 0x" + LevelAreaSNO.ToString("X8"));
            if (HealthPercent.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("HealthPercent.Value: " + HealthPercent.Value.ToString("G"));
            }
            b.Append(' ', pad); b.AppendLine("TrickleInfoType: 0x" + TrickleInfoType.ToString("X8") + " (" + TrickleInfoType + ")");
            b.Append(' ', pad); b.AppendLine("TrickleFlags: 0x" + TrickleFlags.ToString("X8") + " (" + TrickleFlags + ")");
            if (HeadstoneCorpseReviveTime.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("HeadstoneCorpseReviveTime.Value: 0x" + HeadstoneCorpseReviveTime.Value.ToString("X8") + " (" + HeadstoneCorpseReviveTime.Value + ")");
            }
            if (MinimapTextureSNO.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MinimapTextureSNO.Value: 0x" + MinimapTextureSNO.Value.ToString("X8") + " (" + MinimapTextureSNO.Value + ")");
            }
            if (MinimapStringLabel.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MinimapStringLabel.Value: 0x" + MinimapStringLabel.Value.ToString("X8") + " (" + MinimapStringLabel.Value + ")");
            }
            if (MinimapIconLabel.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MinimapIconLabel.Value: 0x" + MinimapIconLabel.Value.ToString("X8") + " (" + MinimapIconLabel.Value + ")");
            }
            if (StringListSNO.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("StringListSNO.Value: 0x" + StringListSNO.Value.ToString("X8") + " (" + StringListSNO.Value + ")");
            }
            if (MinDisplayRangeSq.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MinDisplayRangeSq.Value: " + MinDisplayRangeSq.Value.ToString("G"));
            }
            if (MaxDisplayRangeSq.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MaxDisplayRangeSq.Value: " + MaxDisplayRangeSq.Value.ToString("G"));
            }
            if (Action.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Action.Value: 0x" + Action.Value.ToString("X8") + " (" + Action.Value + ")");
            }
            if (EngagedWithRareTime.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("EngagedWithRareTime.Value: 0x" + EngagedWithRareTime.Value.ToString("X8") + " (" + EngagedWithRareTime.Value + ")");
            }
            if (EngagedWithGoblinTime.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("EngagedWithGoblinTime.Value: 0x" + EngagedWithGoblinTime.Value.ToString("X8") + " (" + EngagedWithGoblinTime.Value + ")");
            }
            if (InCombat.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("InCombat.Value: " + (InCombat.Value ? "true" : "false"));
            }
            if (TieredRiftRank.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("TieredRiftRank.Value: 0x" + TieredRiftRank.Value.ToString("X8") + " (" + TieredRiftRank.Value + ")");
            }
            if (Brawling.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("Brawling.Value: " + (Brawling.Value ? "true" : "false"));
            }
            if (InUberFight.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("InUberFight.Value: " + (InUberFight.Value ? "true" : "false"));
            }
            if (gbidPortraitFrame.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("gbidPortraitFrame.Value: 0x" + gbidPortraitFrame.Value.ToString("X8") + " (" + gbidPortraitFrame.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message(Opcodes.ACDEnterKnownMessage)]
    public class ACDEnterKnownMessage : GameMessage
    {
        public uint ActorID; 
        public int ActorSNOId;
        public int Flags;
        public int LocationType;
        public WorldLocationMessageData WorldLocation;
        public InventoryLocationMessageData InventoryLocation;
        public GBHandle GBHandle;
        public int snoGroup;
        public int snoHandle;
        public int Quality;  
        public byte LookLinkIndex;
        public int? /* sno */ snoAmbientOcclusionOverrideTex;
        public int? MarkerSetSNO;
        public int? MarkerSetIndex;
        public uint? TimeActorCreated = null;
        public EnterKnownLookOverrides EnterKnownLookOverrides;

        public ACDEnterKnownMessage() : base(Opcodes.ACDEnterKnownMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            ActorSNOId = buffer.ReadInt(32);
            Flags = buffer.ReadInt(9);
            LocationType = buffer.ReadInt(2) + (-1);
            if (buffer.ReadBool())
            {
                WorldLocation = new WorldLocationMessageData();
                WorldLocation.Parse(buffer);
            }
            if (buffer.ReadBool())
            {
                InventoryLocation = new InventoryLocationMessageData();
                InventoryLocation.Parse(buffer);
            }
            GBHandle = new GBHandle();
            GBHandle.Parse(buffer);
            snoGroup = buffer.ReadInt(32);
            snoHandle = buffer.ReadInt(32);
            Quality = buffer.ReadInt(4) + (-1);
            LookLinkIndex = (byte)buffer.ReadInt(8);
            if (buffer.ReadBool())
            {
                snoAmbientOcclusionOverrideTex = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MarkerSetSNO = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                MarkerSetIndex = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                TimeActorCreated = buffer.ReadUInt(32);
            }
            if (buffer.ReadBool())
            {
                EnterKnownLookOverrides.Parse(buffer);
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(32, ActorSNOId);
            buffer.WriteInt(9, Flags);
            buffer.WriteInt(2, LocationType - (-1));
            buffer.WriteBool(WorldLocation != null);
            if (WorldLocation != null)
            {
                WorldLocation.Encode(buffer);
            }
            buffer.WriteBool(InventoryLocation != null);
            if (InventoryLocation != null)
            {
                InventoryLocation.Encode(buffer);
            }
            GBHandle.Encode(buffer);
            buffer.WriteInt(32, snoGroup);
            buffer.WriteInt(32, snoHandle);
            buffer.WriteInt(4, Quality - (-1));
            buffer.WriteInt(8, LookLinkIndex);
            buffer.WriteBool(snoAmbientOcclusionOverrideTex.HasValue);
            if (snoAmbientOcclusionOverrideTex.HasValue)
            {
                buffer.WriteInt(32, snoAmbientOcclusionOverrideTex.Value);
            }
            buffer.WriteBool(MarkerSetSNO.HasValue);
            if (MarkerSetSNO.HasValue)
            {
                buffer.WriteInt(32, MarkerSetSNO.Value);
            }
            buffer.WriteBool(MarkerSetIndex.HasValue);
            if (MarkerSetIndex.HasValue)
            {
                buffer.WriteInt(32, MarkerSetIndex.Value);
            }
            buffer.WriteBool(TimeActorCreated.HasValue);
            if (TimeActorCreated.HasValue)
            {
                buffer.WriteUInt(32, TimeActorCreated.Value);
            }
            buffer.WriteBool(EnterKnownLookOverrides != null);
            if (EnterKnownLookOverrides != null)
            {
                EnterKnownLookOverrides.Encode(buffer);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ACDEnterKnownMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("ActorSNOId: 0x" + ActorSNOId.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Flags: 0x" + Flags.ToString("X8") + " (" + Flags + ")");
            b.Append(' ', pad); b.AppendLine("LocationType: 0x" + LocationType.ToString("X8") + " (" + LocationType + ")");
            if (WorldLocation != null)
            {
                WorldLocation.AsText(b, pad);
            }
            if (InventoryLocation != null)
            {
                InventoryLocation.AsText(b, pad);
            }
            GBHandle.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("snoGroup: 0x" + snoGroup.ToString("X8") + " (" + snoGroup + ")");
            b.Append(' ', pad); b.AppendLine("snoHandle: 0x" + snoHandle.ToString("X8") + " (" + snoHandle + ")");
            b.Append(' ', pad); b.AppendLine("Quality: 0x" + Quality.ToString("X8") + " (" + Quality + ")");
            b.Append(' ', pad); b.AppendLine("LookLinkIndex: 0x" + LookLinkIndex.ToString("X2"));
            if (snoAmbientOcclusionOverrideTex.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("snoAmbientOcclusionOverrideTex.Value: 0x" + snoAmbientOcclusionOverrideTex.Value.ToString("X8"));
            }
            if (MarkerSetSNO.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MarkerSetSNO.Value: 0x" + MarkerSetSNO.Value.ToString("X8") + " (" + MarkerSetSNO.Value + ")");
            }
            if (MarkerSetIndex.HasValue)
            {
                b.Append(' ', pad); b.AppendLine("MarkerSetIndex.Value: 0x" + MarkerSetIndex.Value.ToString("X8") + " (" + MarkerSetIndex.Value + ")");
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World
{
    [Message(Opcodes.TargetMessage, Consumers.Player)]
    public class TargetMessage : GameMessage
    {
        public int Type;
        public uint TargetID; // Targeted actor's DynamicID
        public WorldPlace Place;
        public int /* sno */ PowerSNO; // SNO of the power that was used on the targeted actor
        public int annItemBeingUsed;
        public int ComboLevel;
        public AnimPreplayData AnimPreplayData;
        public bool FlagsUsed;
        public int Flags;
        public bool BattlePayClickStatsUsed;
        public float BattlePayClickStats;

        public override void Parse(GameBitBuffer buffer)
        {
            Type = buffer.ReadInt(3) + (-1);
            TargetID = buffer.ReadUInt(32);
            Place = new WorldPlace();
            Place.Parse(buffer);
            PowerSNO = buffer.ReadInt(32);
            annItemBeingUsed = buffer.ReadInt(32);
            ComboLevel = buffer.ReadInt(2);
            if (buffer.ReadBool())
            {
                AnimPreplayData = new AnimPreplayData();
                AnimPreplayData.Parse(buffer);
            }
            if (buffer.ReadBool())
            {
                Flags = buffer.ReadInt(32);
            }
            if (buffer.ReadBool())
            {
                BattlePayClickStats = buffer.ReadFloat32();
            }
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, Type - (-1));
            buffer.WriteUInt(32, TargetID);
            Place.Encode(buffer);
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteInt(32, annItemBeingUsed);
            buffer.WriteInt(2, ComboLevel);
            buffer.WriteBool(AnimPreplayData != null);
            if (AnimPreplayData != null)
            {
                AnimPreplayData.Encode(buffer);
            }
            buffer.WriteBool(FlagsUsed);
            if (FlagsUsed)
            {
                buffer.WriteInt(32, Flags);
            }
            buffer.WriteBool(BattlePayClickStatsUsed);
            if (BattlePayClickStatsUsed)
            {
                buffer.WriteFloat32(BattlePayClickStats);
            }
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("TargetMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Type: 0x" + Type.ToString("X8") + " (" + Type + ")");
            b.Append(' ', pad); b.AppendLine("TargetID: 0x" + TargetID.ToString("X8") + " (" + TargetID + ")");
            Place.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("PowerSNO: 0x" + PowerSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("annItemBeingUsed: 0x" + annItemBeingUsed.ToString("X8") + " (" + annItemBeingUsed + ")");
            b.Append(' ', pad); b.AppendLine("ComboLevel: 0x" + ComboLevel.ToString("X8") + " (" + ComboLevel + ")");
            if (AnimPreplayData != null)
            {
                AnimPreplayData.AsText(b, pad);
            }
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

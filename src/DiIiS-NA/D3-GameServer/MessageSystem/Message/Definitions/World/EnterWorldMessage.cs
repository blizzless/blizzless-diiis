using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using DiIiS_NA.GameServer.Core.Types.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World
{
    [Message(Opcodes.EnterWorldMessage)]
    public class EnterWorldMessage : GameMessage
    {
        public Vector3D EnterPosition;
        public uint WorldID; // World's DynamicID
        public int /* sno */ WorldSNO;
        public int PlayerIndex;
        public bool EnterLookUsed;
        public EnterKnownLookOverrides EnterKnownLookOverrides;

        public EnterWorldMessage() : base(Opcodes.EnterWorldMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            EnterPosition = new Vector3D();
            EnterPosition.Parse(buffer);
            WorldID = buffer.ReadUInt(32);
            WorldSNO = buffer.ReadInt(32);
            PlayerIndex = buffer.ReadInt(32);
            EnterLookUsed = buffer.ReadBool();
            if (EnterLookUsed)
            {
                EnterKnownLookOverrides.Parse(buffer);
            }

        }

        public override void Encode(GameBitBuffer buffer)
        {
            EnterPosition.Encode(buffer);
            buffer.WriteUInt(32, WorldID);
            buffer.WriteInt(32, WorldSNO);
            buffer.WriteInt(32, PlayerIndex);
            buffer.WriteBool(EnterLookUsed);
            if (EnterLookUsed)
            {
                EnterKnownLookOverrides.Encode(buffer);
            }

        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EnterWorldMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            EnterPosition.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', pad); b.AppendLine("WorldSNO: 0x" + WorldSNO.ToString("X8"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

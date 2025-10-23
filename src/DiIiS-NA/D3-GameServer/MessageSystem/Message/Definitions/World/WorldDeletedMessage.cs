using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.World
{
    [Message(Opcodes.WorldDeletedMessage)]
    public class WorldDeletedMessage : GameMessage
    {
        public uint WorldID; // World's DynamicID

        public WorldDeletedMessage() : base(Opcodes.WorldDeletedMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            WorldID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, WorldID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("WorldDeletedMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("WorldID: 0x" + WorldID.ToString("X8") + " (" + WorldID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

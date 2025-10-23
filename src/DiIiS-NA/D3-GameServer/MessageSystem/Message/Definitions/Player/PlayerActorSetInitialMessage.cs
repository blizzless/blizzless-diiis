using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerActorSetInitialMessage)]
    public class PlayerActorSetInitialMessage : GameMessage
    {
        public uint ActorId; // Player's DynamicID
        public int PlayerIndex;

        public PlayerActorSetInitialMessage() : base(Opcodes.PlayerActorSetInitialMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorId = buffer.ReadUInt(32);
            PlayerIndex = buffer.ReadInt(3);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorId);
            buffer.WriteInt(3, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayerActorSetInitialMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorId: 0x" + ActorId.ToString("X8") + " (" + ActorId + ")");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

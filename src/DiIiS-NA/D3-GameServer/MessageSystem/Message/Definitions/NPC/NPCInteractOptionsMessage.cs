using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.NPC
{
    [Message(Opcodes.NPCInteractOptionsMessage)]
    public class NPCInteractOptionsMessage : GameMessage
    {
        public uint ActorID;
        // MaxLength = 20
        public NPCInteraction[] tNPCInteraction;
        public NPCInteractOptionsType Type;

        public NPCInteractOptionsMessage()
            : base(Opcodes.NPCInteractOptionsMessage)
        { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            tNPCInteraction = new NPCInteraction[buffer.ReadInt(5)];
            for (int i = 0; i < tNPCInteraction.Length; i++) { tNPCInteraction[i] = new NPCInteraction(); tNPCInteraction[i].Parse(buffer); }
            Type = (NPCInteractOptionsType)buffer.ReadInt(2);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            int realcount = 0;
            for (int i = 0; i < tNPCInteraction.Length; i++)
                if (tNPCInteraction[i] != null)
                    realcount++;
            buffer.WriteInt(5, realcount);
            for (int i = 0; i < tNPCInteraction.Length; i++) { if(tNPCInteraction[i] != null) tNPCInteraction[i].Encode(buffer); }
            buffer.WriteInt(2, (int)Type);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("NPCInteractOptionsMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("tNPCInteraction:");
            b.Append(' ', pad); b.AppendLine("{");
            //for (int i = 0; i < tNPCInteraction.Length; i++) { tNPCInteraction[i].AsText(b, pad + 1); b.AppendLine(); }
            b.Append(' ', pad); b.AppendLine("}"); b.AppendLine();
            b.Append(' ', pad); b.AppendLine("Type: 0x" + ((int)Type).ToString("X8") + " (" + Type + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }

    public enum NPCInteractOptionsType
    {
        Normal = 0,
        Conversation = 1,
        Unknown2 = 2, // Works like normal? /fasbat
    }
}

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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Act
{
    [Message(Opcodes.ActTransitionMessage)]
    public class ActTransitionMessage : GameMessage
    {
        public int Act;
        public bool OnJoin;

        public ActTransitionMessage() : base(Opcodes.ActTransitionMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Act = buffer.ReadInt(12) + (-1);
            OnJoin = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(12, Act - (-1));
            buffer.WriteBool(OnJoin);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("ActTransitionMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Act: 0x" + Act.ToString("X8") + " (" + Act + ")");
            b.Append(' ', pad); b.AppendLine("OnJoin: " + (OnJoin ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

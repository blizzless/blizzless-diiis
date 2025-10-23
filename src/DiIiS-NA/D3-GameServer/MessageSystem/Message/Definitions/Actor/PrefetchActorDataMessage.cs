using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Actor
{
    [Message(Opcodes.PrefetchActorMessage)]
    public class PrefetchActorDataMessage : GameMessage
    {
        public int /* sno */ Field0;
        public int[] Field1;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(32);
            Field1 = new int[8];
            for (int i = 0; i < Field1.Length; i++)
                Field1[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, Field0);
            for (int i = 0; i < Field1.Length; i++)
                buffer.WriteInt(32, Field1[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

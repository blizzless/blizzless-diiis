using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.PVP
{
    [Message(Opcodes.PVPTokenRedemptionMessage)]
    public class PVPTokenRedemptionMessage : GameMessage
    {
        public int Field0;
        public int[] Field1;
        public int[] Field2;
        public int[] Field3;

        public override void Parse(GameBitBuffer buffer)
        {
            Field0 = buffer.ReadInt(3);
            Field1 = new int[4];
            for (int i = 0; i < Field1.Length; i++)
                Field1[i] = buffer.ReadInt(32);
            Field2 = new int[4];
            for (int i = 0; i < Field2.Length; i++)
                Field2[i] = buffer.ReadInt(32);
            Field3 = new int[4];
            for (int i = 0; i < Field3.Length; i++)
                Field3[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, Field0);
            for (int i = 0; i < Field1.Length; i++)
                buffer.WriteInt(32, Field1[i]);
            for (int i = 0; i < Field2.Length; i++)
                buffer.WriteInt(32, Field2[i]);
            for (int i = 0; i < Field3.Length; i++)
                buffer.WriteInt(32, Field3[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(new[] { Opcodes.PlayerLoadoutEquipResultsMessage })]
    public class PlayerLoadoutEquipResultsMessage : GameMessage
    {
        public bool SocketsChanged;
        public int[] RemovedItemANNs;

        public override void Parse(GameBitBuffer buffer)
        {
            SocketsChanged = buffer.ReadBool();
            RemovedItemANNs = new int[buffer.ReadInt(6)];
            for(int i = 0; i < RemovedItemANNs.Length; i++)
                RemovedItemANNs[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBool(SocketsChanged);
            buffer.WriteInt(6, RemovedItemANNs.Length);
            for (int i = 0; i < RemovedItemANNs.Length; i++)
                buffer.WriteInt(32, RemovedItemANNs[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

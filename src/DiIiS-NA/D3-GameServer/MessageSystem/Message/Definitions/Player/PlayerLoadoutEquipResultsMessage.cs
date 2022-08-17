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

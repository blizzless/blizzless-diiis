using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Player
{
    [Message(Opcodes.PlayerSavedDataMessage)]
    public class PlayerSavedDataMessage : GameMessage
    {
        public PlayerSavedData SavedData;
        public uint PlayerIndex;

        public override void Parse(GameBitBuffer buffer)
        {
            SavedData = new PlayerSavedData();
            SavedData.Parse(buffer);
            PlayerIndex = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            SavedData.Encode(buffer);
            buffer.WriteUInt(32, PlayerIndex);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}

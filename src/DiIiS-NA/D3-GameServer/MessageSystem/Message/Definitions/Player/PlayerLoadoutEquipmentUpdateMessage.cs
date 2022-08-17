//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
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
    [Message(Opcodes.PlayerLoadoutEquipmentUpdateMessage)]
    public class PlayerLoadoutEquipmentUpdateMessage : GameMessage
    {
        public int LoadoutIndex;
        public int EquipmentIndex;
        public LoadoutItemData NewItem;

        public override void Parse(GameBitBuffer buffer)
        {
            LoadoutIndex = buffer.ReadInt(32);
            EquipmentIndex = buffer.ReadInt(32);
            NewItem.Parse(buffer);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, LoadoutIndex);
            buffer.WriteInt(32, EquipmentIndex);
            NewItem.Encode(buffer);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

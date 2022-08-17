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
    [Message(Opcodes.UpdateBannerMessage, Consumers.Player)]
    public class UpdateBannerMessage : GameMessage
    {
        public byte[] Data;

        public UpdateBannerMessage() : base(Opcodes.UpdateBannerMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            Data = buffer.ReadBlob(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteBlob(32, Data);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
        }
    }
}

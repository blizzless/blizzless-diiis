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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD
{
    [Message (new[] { Opcodes.PreloadAddACDMessage, Opcodes.PreloadRemoveACDMessage })]
    public class PreloadACDDataMessage : GameMessage
    {
        public uint ActorID;
        public int /* sno */ SNOActor;
        public int eWeaponClass;
        public int[] /* gbid */ gbidMonsterAffixes;

        public PreloadACDDataMessage(Opcodes id) : base(id) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            SNOActor = buffer.ReadInt(32);
            eWeaponClass = buffer.ReadInt(32);
            gbidMonsterAffixes = new int[8];
            for (int i = 0; i < gbidMonsterAffixes.Length; i++)
                gbidMonsterAffixes[i] = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(32, SNOActor);
            buffer.WriteInt(32, eWeaponClass);
            for (int i = 0; i < gbidMonsterAffixes.Length; i++)
                buffer.WriteInt(32, gbidMonsterAffixes[i]);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            //throw new NotImplementedException();
        }
    }
}

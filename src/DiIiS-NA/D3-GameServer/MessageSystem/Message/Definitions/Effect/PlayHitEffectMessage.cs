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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Effect
{
    [Message(Opcodes.PlayHitEffectMessage)]
    public class PlayHitEffectMessage : GameMessage
    {
        public uint ActorID;
        public uint HitDealer;
        public int DamageType;
        public bool CriticalDamage;

        public PlayHitEffectMessage() : base(Opcodes.PlayHitEffectMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            ActorID = buffer.ReadUInt(32);
            HitDealer = buffer.ReadUInt(32);
            DamageType = buffer.ReadInt(3) + (-1);
            CriticalDamage = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteUInt(32, HitDealer);
            buffer.WriteInt(3, DamageType - (-1));
            buffer.WriteBool(CriticalDamage);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("PlayHitEffectMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', pad); b.AppendLine("HitDealer: 0x" + HitDealer.ToString("X8") + " (" + HitDealer + ")");
            b.Append(' ', pad); b.AppendLine("DamageType: 0x" + DamageType.ToString("X8") + " (" + DamageType + ")");
            b.Append(' ', pad); b.AppendLine("CriticalDamage: " + (CriticalDamage ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }
    }
}

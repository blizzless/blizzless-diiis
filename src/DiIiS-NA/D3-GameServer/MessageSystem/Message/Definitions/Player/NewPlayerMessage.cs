//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields.BlizzLess.Net.GS.Message.Fields;
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
    [Message(Opcodes.NewPlayerMessage)]
    public class NewPlayerMessage : GameMessage
    {
        public int PlayerIndex;
        public long NewToonId;
        public GameAccountHandle GameAccountId;
        public string ToonName;
        public int Team;
        public int Class;
        public int /* sno */ snoActorPortrait;
        public int Level;
        public int AltLevel;
        public int HighestHeroSoloRiftLevel;
        public HeroStateData StateData;
        public bool JustJoined;
        public int Field9;
        public uint ActorID; // Hero's DynamicID

        public NewPlayerMessage() : base(Opcodes.NewPlayerMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            PlayerIndex = buffer.ReadInt(3);
            NewToonId = buffer.ReadInt64(64);
            GameAccountId = new GameAccountHandle();
            GameAccountId.Parse(buffer);
            ToonName = buffer.ReadCharArray(49);
            Team = buffer.ReadInt(5) + (-1);
            Class = buffer.ReadInt(3) + (-1);
            snoActorPortrait = buffer.ReadInt(32);
            Level = buffer.ReadInt(7);
            AltLevel = buffer.ReadInt(15);
            HighestHeroSoloRiftLevel = buffer.ReadInt(9);
            StateData = new HeroStateData();
            StateData.Parse(buffer);
            JustJoined = buffer.ReadBool();
            Field9 = buffer.ReadInt(32);
            ActorID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(3, PlayerIndex);
            buffer.WriteInt64(64, NewToonId);
            GameAccountId.Encode(buffer);
            //GameAccountIdFalse.Encode(buffer);
            buffer.WriteCharArray(49, ToonName);
            buffer.WriteInt(5, Team - (-1));
            buffer.WriteInt(3, Class - (-1));
            buffer.WriteInt(32, snoActorPortrait);
            buffer.WriteInt(7, Level);
            buffer.WriteInt(15, AltLevel);
            buffer.WriteInt(9, HighestHeroSoloRiftLevel);
            StateData.Encode(buffer);
            buffer.WriteBool(JustJoined);
            buffer.WriteInt(32, Field9);
            buffer.WriteUInt(32, ActorID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("NewPlayerMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("PlayerIndex: 0x" + PlayerIndex.ToString("X8") + " (" + PlayerIndex + ")");
            GameAccountId.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("ToonName: \"" + ToonName + "\"");
            b.Append(' ', pad); b.AppendLine("Team: 0x" + Team.ToString("X8") + " (" + Team + ")");
            b.Append(' ', pad); b.AppendLine("Class: 0x" + Class.ToString("X8") + " (" + Class + ")");
            b.Append(' ', pad); b.AppendLine("snoActorPortrait: 0x" + snoActorPortrait.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Level: 0x" + Level.ToString("X8") + " (" + Level + ")");
            b.Append(' ', pad); b.AppendLine("AltLevel: 0x" + AltLevel.ToString("X8") + " (" + AltLevel + ")");
            b.Append(' ', pad); b.AppendLine("HighestHeroSoloRiftLevel: " + (HighestHeroSoloRiftLevel.ToString("X8")+ " (" + HighestHeroSoloRiftLevel + ")"));
            //StateData.AsText(b, pad);
            b.Append(' ', pad); b.AppendLine("JustJoined: " + (JustJoined ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Field9: 0x" + Field9.ToString("X8") + " (" + Field9 + ")");
            b.Append(' ', pad); b.AppendLine("ActorID: 0x" + ActorID.ToString("X8") + " (" + ActorID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

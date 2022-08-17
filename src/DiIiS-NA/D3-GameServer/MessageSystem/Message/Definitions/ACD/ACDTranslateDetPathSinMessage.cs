//Blizzless Project 2022 
using DiIiS_NA.GameServer.MessageSystem.Message.Fields;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
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
    [Message(Opcodes.ACDTranslateDetPathSinMessage)]
    public class ACDTranslateDetPathSinMessage : GameMessage
    {
        public uint ActorID;
        public int DPath;
        public int Seed;
        public int Carry;
        public Vector3D TargetPostition;
        public float /* angle */ Angle;
        public Vector3D StartPosition;
        public int MoveFlags;
        public int AnimTag;
        public int /* sno */ PowerSNO;
        public int Var0Int;
        public float Var0Fl;
        public DPathSinData SinData;
        public float SpeedMulti;
        

        public ACDTranslateDetPathSinMessage() : base(Opcodes.ACDTranslateDetPathSinMessage) { }



        public override void Parse(GameBitBuffer buffer)
        {

        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteUInt(32, ActorID);
            buffer.WriteInt(4, DPath);
            buffer.WriteInt(32, Seed);
            buffer.WriteInt(32, Carry);
            TargetPostition.Encode(buffer);
            buffer.WriteFloat32(Angle);
            StartPosition.Encode(buffer);
            buffer.WriteInt(32, MoveFlags);
            buffer.WriteInt(32, AnimTag);
            buffer.WriteInt(32, PowerSNO);
            buffer.WriteInt(32, Var0Int);
            buffer.WriteFloat32(Var0Fl);
            SinData.Encode(buffer);
            buffer.WriteFloat32(SpeedMulti);
            
        }

        public override void AsText(StringBuilder b, int pad)
        {
            
        }


    }
}

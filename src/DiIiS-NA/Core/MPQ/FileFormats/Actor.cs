//Blizzless Project 2022
using CrystalMpq;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.GameServer.Core.Types.Collision;
using Gibbed.IO;
using System.Collections.Generic;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
using DiIiS_NA.GameServer.Core.Types.TagMap;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Actor)]
    public class Actor : FileFormat
    {
        public Header Header { get; private set; }
        public int Flags { get; private set; }
        public ActorType Type { get; private set; }
        public int ApperanceSNO { get; private set; }
        public int PhysMeshSNO { get; private set; }
        public AxialCylinder Cylinder { get; private set; }
        public Sphere Sphere { get; private set; }
        public AABB AABBBounds { get; private set; }
        public TagMap TagMap { get; private set; }
        public int AnimSetSNO { get; private set; }
        public int AniimTreeSno { get; private set; }
        public int MonsterSNO { get; private set; }
        public List<MsgTriggeredEvent> MsgTriggeredEvents = new List<MsgTriggeredEvent>();
        public int MsgTriggeredEventCount { get; private set; }
        public Vector3D LocationPowerSrc { get; private set; }
        public WeightedLook[] Looks { get; private set; }
        public int PhysicsSNO { get; private set; }
        public int PhysicsFlags { get; private set; }
        public int Material { get; private set; }
        public float ExplosiionFactor { get; private set; }
        public float WindFactor { get; private set; }
        public float PartialRagdollResponsiveness { get; private set; }
        public ActorCollisionData ActorCollisionData { get; private set; }
        public int[] InventoryImages { get; private set; }
        public int SocketedImage { get; private set; }
        public string CastingNotes { get; private set; }
        public string VoiceOverRole { get; private set; }

        public Actor(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            //+16
            this.Flags = stream.ReadValueS32();
            this.Type = (ActorType)stream.ReadValueS32();
            this.ApperanceSNO = stream.ReadValueS32();
            this.PhysMeshSNO = stream.ReadValueS32();
            this.Cylinder = new AxialCylinder(stream);
            this.Sphere = new Sphere(stream);
            this.AABBBounds = new AABB(stream);

            this.TagMap = stream.ReadSerializedItem<TagMap>();
            stream.Position += (2 * 4);

            this.AnimSetSNO = stream.ReadValueS32();
            this.MonsterSNO = stream.ReadValueS32();
            //stream.Position += 8;
            MsgTriggeredEvents = stream.ReadSerializedData<MsgTriggeredEvent>();
            this.AniimTreeSno = stream.ReadValueS32();
            //stream.Position += 4;
            //this.IntNew = stream.ReadValueS32();
            //stream.Position += 8;

            this.MsgTriggeredEventCount = MsgTriggeredEvents.Count;
            stream.Position += 12;
            this.LocationPowerSrc = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());

            this.Looks = new WeightedLook[8];
            for (int i = 0; i < 8; i++)
            {
                this.Looks[i] = new WeightedLook(stream);
            }

            this.PhysicsSNO = stream.ReadValueS32();
            this.PhysicsFlags = stream.ReadValueS32();
            this.Material = stream.ReadValueS32();
            this.ExplosiionFactor = stream.ReadValueF32();
            this.WindFactor = stream.ReadValueF32();
            this.PartialRagdollResponsiveness = stream.ReadValueF32();

            this.ActorCollisionData = new ActorCollisionData(stream);

            this.InventoryImages = new int[7]; //Was 5*8/4 - Darklotus
            for (int i = 0; i < InventoryImages.Length; i++)
            {
                this.InventoryImages[i] = stream.ReadValueS32();
            }
            stream.Position += (4 * 7);
            this.SocketedImage = stream.ReadValueS32();
            stream.Position += (4 * 5);
            CastingNotes = stream.ReadSerializedString();
            VoiceOverRole = stream.ReadSerializedString();

            // Updated based on BoyC's 010 template and Moack's work. Think we just about read all data from actor now.- DarkLotus
            stream.Close();
        }
    }
    public class ActorCollisionData
    {
        public ActorCollisionFlags CollFlags { get; private set; }
        public int CollisiionShape { get; private set; }
        public AxialCylinder Cylinder { get; private set; }
        public AABB AABBCollision { get; private set; }
        public float MovingRadiusScalar { get; private set; }

        public ActorCollisionData(MpqFileStream stream)
        {
            CollFlags = new ActorCollisionFlags(stream);
            CollisiionShape = stream.ReadValueS32();
            Cylinder = new AxialCylinder(stream);
            AABBCollision = new AABB(stream);
            MovingRadiusScalar = stream.ReadValueF32();
            //stream.ReadValueS32();// Testing - DarkLotus
        }
    }

    public class ActorCollisionFlags
    {
        public int I0 { get; private set; }
        public int I1 { get; private set; }
        public int I2 { get; private set; }
        public int I3 { get; private set; }

        public ActorCollisionFlags(MpqFileStream stream)
        {
            this.I0 = stream.ReadValueS32();
            this.I1 = stream.ReadValueS32();
            this.I2 = stream.ReadValueS32();
            this.I3 = stream.ReadValueS32();
        }
    }

    public class AxialCylinder
    {
        public Vector3D Position { get; private set; }
        public float Ax1 { get; private set; }
        public float Ax2 { get; private set; }

        public AxialCylinder(MpqFileStream stream)
        {
            this.Position = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            Ax1 = stream.ReadValueF32();
            Ax2 = stream.ReadValueF32();
        }
    }

    public class Sphere
    {
        public Vector3D Position { get; private set; }
        public float Radius { get; private set; }

        public Sphere(MpqFileStream stream)
        {
            Position = new Vector3D(stream.ReadValueF32(), stream.ReadValueF32(), stream.ReadValueF32());
            Radius = stream.ReadValueF32();
        }
    }

    public class WeightedLook
    {
        public string LookLink { get; private set; }
        public int Int0 { get; private set; }

        public WeightedLook(MpqFileStream stream)
        {
            this.LookLink = stream.ReadString(64, true);
            Int0 = stream.ReadValueS32();
        }
    }
}

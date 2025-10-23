using CrystalMpq;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;
using System.Collections.Generic;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Anim)]
    public class Anim : FileFormat
    {
        public Header Header { get; private set; }
        public int Flags { get; private set; }
        public int PlaybackMode { get; private set; }
        public int SNOAppearance { get; private set; }
        public List<AnimPermutation> Permutations { get; private set; }
        public int PermutationCount { get; private set; }
        public int MachineTime { get; private set; }

        public Anim(MpqFile file)
        {
            var stream = file.Open();
            Header = new Header(stream);
            Flags = stream.ReadValueS32();
            PlaybackMode = stream.ReadValueS32();
            SNOAppearance = stream.ReadValueS32();
            Permutations = stream.ReadSerializedData<AnimPermutation>();
            PermutationCount = stream.ReadValueS32();
            stream.Position += 12;
            MachineTime = stream.ReadValueS32();
            stream.Close();
        }
    }

    public class AnimPermutation : ISerializableData
    {
        public int Flags { get; private set; }
        public string PermutationName { get; private set; }
        public float FrameRate { get; private set; }
        public float Compression { get; private set; }
        public float TramslationCompressionRatio { get; private set; }
        public float RotationComressionRatio { get; private set; }
        public float ScaleCompressionRatio { get; private set; }
        public int BlendTime { get; private set; }
        public int FromPermBlendTime { get; private set; }
        public int Weight { get; private set; }
        public float SpeedMultMin { get; private set; }
        public float SpeedMultDelta { get; private set; }
        public float RagdollVelocityFactor { get; private set; }
        public float RagdollMomentumFactor { get; private set; }
        public int BoneNameCount { get; private set; }
        public List<BoneName> BoneNames { get; private set; }
        public int KeyframePosCount { get; private set; }
        public List<TranslationCurve> TranslationCurves { get; private set; }
        public List<RotationCurve> RotationCurves { get; private set; }
        public List<ScaleCurve> ScaleCurves { get; private set; }
        public float ContactKeyframe0 { get; private set; }
        public float ContactKeyframe1 { get; private set; }
        public float ContactKeyframe2 { get; private set; }
        public float ContactKeyframe3 { get; private set; }
        public Vector3D ContactOffset0 { get; private set; }
        public Vector3D ContactOffset1 { get; private set; }
        public Vector3D ContactOffset2 { get; private set; }
        public Vector3D ContactOffset3 { get; private set; }
        public float EarliestInterruptKeyFrame { get; private set; }
        public int KeyedAttachmentsCount { get; private set; }
        public List<KeyframedAttachment> KeyedAttachments { get; private set; }
        public List<Vector3D> KeyframePosList { get; private set; }
        public List<Vector3D> NonlinearOffset { get; private set; }
        public VelocityVector3D AvgVelocity { get; private set; }
        public HardPointLink HardPointLink { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Flags = stream.ReadValueS32();
            PermutationName = stream.ReadString(65, true);
            stream.Position += 3;
            FrameRate = stream.ReadValueF32();
            Compression = stream.ReadValueF32();
            TramslationCompressionRatio = stream.ReadValueF32();
            RotationComressionRatio = stream.ReadValueF32();
            ScaleCompressionRatio = stream.ReadValueF32();
            BlendTime = stream.ReadValueS32();
            FromPermBlendTime = stream.ReadValueS32();
            Weight = stream.ReadValueS32();
            SpeedMultMin = stream.ReadValueF32();
            SpeedMultDelta = stream.ReadValueF32();
            RagdollVelocityFactor = stream.ReadValueF32();
            RagdollMomentumFactor = stream.ReadValueF32();
            BoneNameCount = stream.ReadValueS32();
            BoneNames = stream.ReadSerializedData<BoneName>();
            stream.Position += 12;
            KeyframePosCount = stream.ReadValueS32();
            TranslationCurves = stream.ReadSerializedData<TranslationCurve>();
            stream.Position += 12;
            RotationCurves = stream.ReadSerializedData<RotationCurve>();
            stream.Position += 8;
            ScaleCurves = stream.ReadSerializedData<ScaleCurve>();
            stream.Position += 8;
            ContactKeyframe0 = stream.ReadValueF32();
            ContactKeyframe1 = stream.ReadValueF32();
            ContactKeyframe2 = stream.ReadValueF32();
            ContactKeyframe3 = stream.ReadValueF32();
            ContactOffset0 = new Vector3D(stream);
            ContactOffset1 = new Vector3D(stream);
            ContactOffset2 = new Vector3D(stream);
            ContactOffset3 = new Vector3D(stream);
            EarliestInterruptKeyFrame = stream.ReadValueF32();
            KeyedAttachments = stream.ReadSerializedData<KeyframedAttachment>();
            KeyedAttachmentsCount = stream.ReadValueS32();
            stream.Position += 8;
            KeyframePosList = stream.ReadSerializedData<Vector3D>();
            stream.Position += 8;
            NonlinearOffset = stream.ReadSerializedData<Vector3D>();
            stream.Position += 8;
            AvgVelocity = new VelocityVector3D(stream);
            HardPointLink = new HardPointLink(stream);
            //this.S0 = stream.ReadString(256, true);
            //this.S1 = stream.ReadString(256, true);
            stream.Position += 8;
        }
    }

    public class BoneName : ISerializableData
    {
        public string Name { get; private set; }

        public void Read(MpqFileStream stream)
        {
            Name = stream.ReadString(64, true);
        }
    }

    public class TranslationCurve : ISerializableData
    {
        public int I0 { get; private set; }
        public List<TranslationKey> Keys { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Keys = stream.ReadSerializedData<TranslationKey>();
        }
    }

    public class RotationCurve : ISerializableData
    {
        public int I0 { get; private set; }
        public List<RotationKey> Keys { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Keys = stream.ReadSerializedData<RotationKey>();
        }
    }

    public class ScaleCurve : ISerializableData
    {
        public int I0 { get; private set; }
        public List<ScaleKey> Keys { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Keys = stream.ReadSerializedData<ScaleKey>();
        }
    }

    public class TranslationKey : ISerializableData
    {
        public int I0 { get; private set; }
        public Vector3D Location { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Location = new Vector3D(stream);
        }
    }

    public class RotationKey : ISerializableData
    {
        public int I0 { get; private set; }
        public Quaternion16 Q0 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Q0 = new Quaternion16(stream);
        }
    }

    public class ScaleKey : ISerializableData
    {
        public int I0 { get; private set; }
        public float Scale { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            Scale = stream.ReadValueF32();
        }
    }

    public class KeyframedAttachment : ISerializableData
    {
        public float KeyframeIndex { get; private set; }
        public TriggerEvent Event { get; private set; }

        public void Read(MpqFileStream stream)
        {
            KeyframeIndex = stream.ReadValueF32();
            Event = new TriggerEvent(stream);
        }
    }

    public class VelocityVector3D
    {
        public float VelocityX { get; private set; }
        public float VelocityY { get; private set; }
        public float VelocityZ { get; private set; }

        public VelocityVector3D(MpqFileStream stream)
        {
            VelocityX = stream.ReadValueF32();
            VelocityY = stream.ReadValueF32();
            VelocityZ = stream.ReadValueF32();
        }
    }

    public class Quaternion16
    {
        public short Short0;
        public short Short1;
        public short Short2;
        public short Short3;

        public Quaternion16() { }

        /// <summary>
        /// Reads Quaternion16 from given MPQFileStream.
        /// </summary>
        /// <param name="stream">The MPQFileStream to read from.</param>
        public Quaternion16(MpqFileStream stream)
        {
            Short0 = stream.ReadValueS16();
            Short1 = stream.ReadValueS16();
            Short2 = stream.ReadValueS16();
            Short3 = stream.ReadValueS16();
        }
    }
}

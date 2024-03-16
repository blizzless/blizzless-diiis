using CrystalMpq;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.GameServer.Core.Types.Misc;
using DiIiS_NA.GameServer.Core.Types.SNO;
using Gibbed.IO;
using System.Text;

namespace DiIiS_NA.Core.MPQ.FileFormats.Types
{
    public class Header
    {
        public int DeadBeef { get; private set; }
        public int SnoType { get; private set; }
        public int Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public int SNOId { get; private set; }

        public Header(MpqFileStream stream)
        {
            DeadBeef = stream.ReadValueS32();
            SnoType = stream.ReadValueS32();
            Unknown1 = stream.ReadValueS32();
            Unknown2 = stream.ReadValueS32();
            SNOId = stream.ReadValueS32();
            Unknown3 = stream.ReadValueS32();
            Unknown4 = stream.ReadValueS32();
        }
    }

    public class ScriptFormula
    {
        [PersistentProperty("I0")]
        public int I0 { get; private set; }

        [PersistentProperty("I1")]
        public int I1 { get; private set; }

        [PersistentProperty("I2")]
        public int I2 { get; private set; }

        [PersistentProperty("I3")]
        public int I3 { get; private set; }

        [PersistentProperty("I4")]
        public int I4 { get; private set; }

        [PersistentProperty("NameSize")]
        public int NameSize { get; private set; }

        [PersistentProperty("I5")]
        public int I5 { get; private set; }

        [PersistentProperty("OpcodeSize")]
        public int OpcodeSize { get; private set; }

        [PersistentProperty("OpCodeName")]
        public string OpCodeName { get; private set; }

        [PersistentProperty("OpCodeArray", -1)]
        public byte[] OpCodeArray { get; private set; }

        public ScriptFormula() { }

        public ScriptFormula(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
            I2 = stream.ReadValueS32();
            I3 = stream.ReadValueS32();
            I4 = stream.ReadValueS32();
            NameSize = stream.ReadValueS32();
            I5 = stream.ReadValueS32();
            OpcodeSize = stream.ReadValueS32();
            OpCodeName = stream.ReadStringZ(Encoding.ASCII);

            switch (NameSize % 4)
            {
                case 0:
                    break;
                case 1:
                    stream.Position += 3;
                    break;
                case 2:
                    stream.Position += 2;
                    break;
                case 3:
                    stream.Position += 1;
                    break;

            }
            OpCodeArray = new byte[OpcodeSize];
            stream.Read(OpCodeArray, 0, OpcodeSize);
        }

        public override string ToString()
        {
            return OpCodeName;
        }
    }

    public class ScriptFormulaDetails : ISerializableData
    {
        public string CharArray1 { get; private set; }
        public string CharArray2 { get; private set; }
        public int I0 { get; private set; }
        public int I1 { get; private set; }

        public void Read(MpqFileStream stream)
        {
            CharArray1 = stream.ReadString(256, true);
            CharArray2 = stream.ReadString(512, true);
            I0 = stream.ReadValueS32();
            I1 = stream.ReadValueS32();
        }
    }

    // Replace each Look with just a chararay? DarkLotus
    public class HardPointLink
    {
        public string Name { get; private set; }
        public int I0 { get; private set; }

        public HardPointLink(MpqFileStream stream)
        {
            Name = stream.ReadString(64, true);
            I0 = stream.ReadValueS32();
        }
    }

    public class TriggerConditions
    {
        public enum eMaterial
        {
            DEFAULT_MATERIAL = 0,
            METAL_MATERIAL = 1,
            WOOD_MATERIAL = 2,
            BONE_MATERIAL = 3,
            FLESH_MATERIAL = 4,
            WATER_MATERIAL = 5,
            STONE_MATERIAL = 6,
            GLASS_MATERIAL = 7,
            PAPER_MATERIAL = 8,
            METAL2_MATERIAL = 9,
            METAL3_MATERIAL = 10,
            WOOD2_MATERIAL = 11,
            WOOD3_MATERIAL = 12,
            STONE2_MATERIAL = 13,
            STONE3_MATERIAL = 14,
            BONE2_MATERIAL = 15,
            BONE3_MATERIAL = 16,
            INVALID_MATERIAL = -1
        }
        public int Percent { get; private set; } //0-255
        public int DelayMin { get; private set; }
        public int DelayMax { get; private set; }
        public int RepeatTimeMin { get; private set; }
        public int RepeatTimeDelta { get; private set; }
        public float ImpulseMin { get; private set; }
        public float ImpulseDelta { get; private set; }
        public eMaterial Material { get; private set; }
        public int ConditionFlags { get; private set; }

        public TriggerConditions(MpqFileStream stream)
        {
            Percent = stream.ReadByte();
            stream.Position += 3;
            DelayMin = stream.ReadValueS32();
            DelayMax = stream.ReadValueS32();
            RepeatTimeMin = stream.ReadValueS32();
            RepeatTimeDelta = stream.ReadValueS32();
            ImpulseMin = stream.ReadValueF32();
            ImpulseDelta = stream.ReadValueF32();
            Material = (eMaterial)stream.ReadValueS32();
            ConditionFlags = stream.ReadValueS32();
        }
    }

    public class TriggerEvent
    {
        public enum TriggeredEventType
        {
            ADD_OBJECT = 0,
            DETACH_OBJECT = 1,
            ADD_CLOTH_DEPRECATED = 2,
            DELETE_CLOTH_DEPRECATED = 3,
            ADD_TRAIL = 4,
            DELETE_TRAIL_DEPRECATED = 5,
            MSG = 6,
            DELETE_OBJECT = 7,
            FOOTSTEP = 8,
            ANIM = 9,
            TRANSPARENCY = 10,
            ADD_ROPE = 11,
            DELETE_ROPE_DEPRECATED = 12,
            CHANGE_LOOK = 13,
            SHADOW = 14,
            CHANGE_PHYSICS_DEPRECATED = 15,
            ADD_EFFECT_GROUP = 16,
            DELETE_EFFECT_GROUP_DEPRECATED = 17,
            FROST_BREATH = 18,
            ENABLE_RIGID_ANIMATION_ANGVEL_DEPRECATED = 19,
            DISABLE_RIGID_ANIMATION_ANGVEL_DEPRECATED = 20,
            ENABLE_RIGID_ANIMATION_BOB_DEPRECATED = 21,
            DISABLE_RIGID_ANIMATION_BOB_DEPRECATED = 22,
            MUSIC = 23,
            CAPTURE_BY_PARTICLE_SYSTEM_DEPRECATED = 24,
            SPAWN_OBJECT = 25,
            OUTRO_OBJECT = 26,
            ENABLE_RAGDOLL = 27,
            DISABLE_CONSTRAINT = 28,
            DISABLE_COLLISION_MESSAGES = 29,
            ATTACH_SPAWNED_ACTOR = 30,
            CANCEL_LOOK = 31,
            HITFLASH = 32,
            ENABLE_RAGDOLL_ANIMATION_OVERRIDE = 33,
            DISABLE_RAGDOLL_ANIMATION_OVERRIDE = 34,
            ATTACH_CAMERA = 35,
            FRAMEOUT_ENABLE = 36,
            FRAMEOUT_DISABLE = 37,
            RESTORE_CAMERA = 38,
            FADE = 39,
            NULL = 40,
            OUTRO_ANIMATION = 41,
            HIDE_HELM_AND_SHOULDERS = 42
        }

        public TriggeredEventType eType { get; private set; }
        public TriggerConditions TriggerConditions { get; private set; }
        public int AttachmentType { get; private set; }
        public SNOHandle SNOHandle { get; private set; }
        public int ID { get; private set; }
        public int PinFlags { get; private set; }
        public int RuneType { get; private set; }
        public int UseRuneType { get; private set; }
        public HardPointLink[] HardPointLinks { get; private set; }
        public string LookLink { get; private set; }
        public string ConstraintLink { get; private set; }
        public int AnimTag { get; private set; }
        public float Alpha { get; private set; }
        public int MsgPassMethod { get; private set; }
        public int MsgKey { get; private set; }
        public int Trigger { get; private set; }
        public int I8 { get; private set; }
        public int I9 { get; private set; }
        public float F1 { get; private set; }
        public float F2 { get; private set; }
        public int I10 { get; private set; }
        public float F3 { get; private set; }
        public int I11 { get; private set; }
        public float Velocity { get; private set; }
        public int I12 { get; private set; }
        public int SpawnedActorLife { get; private set; } // DT_TIME
        public RGBAColor ColorAdd { get; private set; }
        public int HitFlashAddDuration { get; private set; } // DT_TIME
        public RGBAColor ColorMul { get; private set; }
        public int HitFlashMulDuration { get; private set; } // DT_TIME

        public TriggerEvent(MpqFileStream stream)
        {
            eType = (TriggeredEventType)stream.ReadValueS32();
            TriggerConditions = new TriggerConditions(stream);
            AttachmentType = stream.ReadValueS32();
            SNOHandle = new SNOHandle(stream);
            ID = stream.ReadValueS32();
            PinFlags = stream.ReadValueS32();
            RuneType = stream.ReadValueS32();
            UseRuneType = stream.ReadValueS32();
            HardPointLinks = new HardPointLink[2];
            HardPointLinks[0] = new HardPointLink(stream);
            HardPointLinks[1] = new HardPointLink(stream);
            LookLink = stream.ReadString(64, true);
            ConstraintLink = stream.ReadString(64, true);
            AnimTag = stream.ReadValueS32();
            Alpha = stream.ReadValueF32();
            MsgPassMethod = stream.ReadValueS32();
            MsgKey = stream.ReadValueS32();
            Trigger = stream.ReadValueS32();
            I8 = stream.ReadValueS32();
            I9 = stream.ReadValueS32();
            F1 = stream.ReadValueF32();
            F2 = stream.ReadValueF32();
            I10 = stream.ReadValueS32();
            F3 = stream.ReadValueF32();
            I11 = stream.ReadValueS32();
            Velocity = stream.ReadValueF32();
            I12 = stream.ReadValueS32();
            SpawnedActorLife = stream.ReadValueS32();
            ColorAdd = new RGBAColor(stream);
            HitFlashAddDuration = stream.ReadValueS32();
            ColorMul = new RGBAColor(stream);
            HitFlashMulDuration = stream.ReadValueS32();
        }
    }

    public class MsgTriggeredEvent : ISerializableData
    {
        public int I0 { get; private set; }
        public TriggerEvent TriggerEvent { get; private set; }

        public void Read(MpqFileStream stream)
        {
            I0 = stream.ReadValueS32();
            TriggerEvent = new TriggerEvent(stream);
        }
    }


    public class ItemSpecifierData
    {
        [PersistentProperty("ItemGBId")]
        public int ItemGBId { get; private set; }

        [PersistentProperty("I0")]
        public int NumAffixes { get; private set; }

        [PersistentProperty("GBIdAffixes", 3)]
        public int[] GBIdAffixes { get; private set; }

        [PersistentProperty("I1")]
        public int AdditionalRandomAffixes { get; private set; }

        [PersistentProperty("I2")]
        public int AdditionalRandomAffixesDelta { get; private set; }

        public int MinLegendaryAffixes { get; private set; }
        public int AccountBound { get; private set; }
        public int AdditionalMajorAffixes { get; private set; }
        public int AdditionalMinorAffixes { get; private set; }

        public ItemSpecifierData() { }

        public ItemSpecifierData(MpqFileStream stream)
        {
            ItemGBId = stream.ReadValueS32();
            NumAffixes = stream.ReadValueS32();
            GBIdAffixes = new int[6];
            for (int i = 0; i < GBIdAffixes.Length; i++)
            {
                GBIdAffixes[i] = stream.ReadValueS32();
            }
            AdditionalRandomAffixes = stream.ReadValueS32();
            AdditionalRandomAffixesDelta = stream.ReadValueS32();
            MinLegendaryAffixes = stream.ReadValueS32();
            AccountBound = stream.ReadValueS32();
            AdditionalMajorAffixes = stream.ReadValueS32();
            AdditionalMinorAffixes = stream.ReadValueS32();


        }
    }

}

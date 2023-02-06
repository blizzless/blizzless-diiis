using System.Collections.Generic;
using System.Text;
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using Newtonsoft.Json;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Conversation)]
    public class Conversation : FileFormat
    {
        public Header Header { get; private set; }
        public ConversationTypes ConversationType { get; private set; }
        public int ConversationIcon { get; private set; }
        public int snoConvPiggyback { get; private set; }
        public int[] snoConvUnlocks { get; private set; }
        public int Flags { get; private set; }
        public string SetPlayerFlags { get; private set; }
        public int SNOPrimaryNpc { get; private set; }
        public int SNOAltNpc1 { get; private set; }
        public int SNOAltNpc2 { get; private set; }
        public int SNOAltNpc3 { get; private set; }
        public int SNOAltNpc4 { get; private set; }
        public int LineUID { get; private set; }              // not total nodes :-(
        public List<ConversationTreeNode> RootTreeNodes { get; private set; }
        public byte[] CompiiledScript { get; private set; }
        public int SNOBossEncounter { get; private set; }
        public int Pad { get; private set; }

        public Conversation(MpqFile file)
        {
            MpqFileStream stream = file.Open();

            Header = new Header(stream); //0
            //+16
            ConversationType = (ConversationTypes)stream.ReadValueS32(); //12
            ConversationIcon = stream.ReadValueS32(); //16
            snoConvPiggyback = stream.ReadValueS32(); //20

            snoConvUnlocks = new int[3];
            for (int i = 0; i < snoConvUnlocks.Length; i++) //24
                snoConvUnlocks[i] = stream.ReadValueS32();

            Flags = stream.ReadValueS32(); //36
            SetPlayerFlags = stream.ReadString(128, true); //40
            SNOPrimaryNpc = stream.ReadValueS32(); //168
            SNOAltNpc1 = stream.ReadValueS32(); //172
            SNOAltNpc2 = stream.ReadValueS32(); //176
            SNOAltNpc3 = stream.ReadValueS32(); //180
            SNOAltNpc4 = stream.ReadValueS32(); //184
            LineUID = stream.ReadValueS32(); //188-192
            stream.Position += 8;
            RootTreeNodes = stream.ReadSerializedData<ConversationTreeNode>(); // 200
            stream.Position = stream.Position;
            CompiiledScript = stream.ReadSerializedByteArray(); //216
            stream.Position = stream.Position;
            SNOBossEncounter = stream.ReadValueS32(); //264
            Pad = stream.ReadValueS32(); //268
            stream.Close();
        }

        public string AsText(string filename)
        {
            StringBuilder s = new StringBuilder();

            s.AppendLine(Header.SNOId + ":" + filename);
            s.AppendLine("ConversationType:" + ConversationType);
            s.Append("ConversationIcon:" + ConversationIcon + "   ");
            s.Append("snoConvPiggyback:" + snoConvPiggyback + "   ");
            s.Append("snoConvUnlocks:" + snoConvUnlocks + "   ");
            s.Append("Flags:" + Flags + "   ");
            s.Append("LineUID:" + LineUID + "   ");
            s.Append("Pad:" + Pad + "   ");
            //s.AppendLine("I6:" + I6);

            //s.AppendLine("SNOQuest:" + SNOQuest);
            //s.AppendLine("SNOConvPiggyBack:" + SNOConvPiggyback);
            //s.AppendLine("SNOConvUnlock:" + SNOConvUnlock);
            //s.AppendLine("CompiledScript:" + (CompiledScript.Length != 0).ToString());

            foreach (var node in RootTreeNodes)
                node.AsText(s, 0);
            return s.ToString();
        }
    }


    public class ConversationTreeNode : ISerializableData
    {
        public int ConvNodeType { get; private set; }
        public int Flags { get; private set; }
        public int LineID { get; private set; }              // clasid ? 
        public Speaker LineSpeaker { get; private set; }
        public Speaker SpeakerTarget { get; private set; }
        public int AnimationTag { get; private set; }
        public int Gender { get; private set; }
        public int PlayerClass { get; private set; }
        public int DisplayTimeAdjustment { get; private set; }
        public int ClassFilter { get; private set; }        // only used on nodes with i0 == 5, selects the displaylocalconvline
        public ConvLocalDisplayTimes[] CompressedDisplayTimes = new ConvLocalDisplayTimes[19];
        //public string Comment { get; private set; }
        public int GBIDConvVarCheck { get; private set; }
        public TypeConv ConvVarCheckOp { get; private set; }
        public int ConvVarCheckVal { get; private set; }
        public int GBIDConvVarSet { get; private set; }
        public Ref ConvVarSetOp { get; private set; }
        public int ConvVarSetVal { get; private set; }
        public int BranchIndex { get; private set; }
        public int Weight { get; private set; }

        public List<ConversationTreeNode> TrueNodes { get; private set; }
        public List<ConversationTreeNode> FalseNodes { get; private set; }
        public List<ConversationTreeNode> ChildNodes { get; private set; }

        public void Read(MpqFileStream stream)
        {
            //288
            ConvNodeType = stream.ReadValueS32();
            Flags = stream.ReadValueS32();
            LineID = stream.ReadValueS32();
            LineSpeaker = (Speaker)stream.ReadValueS32();
            SpeakerTarget = (Speaker)stream.ReadValueS32();
            AnimationTag = stream.ReadValueS32();
            Gender = stream.ReadValueS32();
            PlayerClass = stream.ReadValueS32();
            DisplayTimeAdjustment = stream.ReadValueS32();
            ClassFilter = stream.ReadValueS32();
            for (int i = 0; i < CompressedDisplayTimes.Length; i++) //40 //328
                CompressedDisplayTimes[i] = new ConvLocalDisplayTimes(stream);
            GBIDConvVarCheck = stream.ReadValueS32(); //1104

            ConvVarCheckOp = (TypeConv)stream.ReadValueS32();
            ConvVarCheckVal = stream.ReadValueS32();
            GBIDConvVarSet = stream.ReadValueS32();
            ConvVarSetOp = (Ref)stream.ReadValueS32(); //1408
            ConvVarSetVal = stream.ReadValueS32();
            BranchIndex = stream.ReadValueS32();
            Weight = stream.ReadValueS32();
            //strea3.Position += 4;       // these are unaccounted for...xml offsets just skips ahead
            stream.Position += (2 * 4);
            TrueNodes = stream.ReadSerializedData<ConversationTreeNode>();
            stream.Position += (2 * 4);
            FalseNodes = stream.ReadSerializedData<ConversationTreeNode>();
            stream.Position += (2 * 4);
            ChildNodes = stream.ReadSerializedData<ConversationTreeNode>();
        }

        public void AsText(StringBuilder s, int pad)
        {
            s.Append(' ', pad);
            s.Append("ConvNodeType:" + ConvNodeType + "   ");
            s.Append("Flags:" + Flags + "   ");
            s.Append("LineID:" + LineID + "   ");
            s.Append("AnimationTag:" + AnimationTag + "   ");
            s.Append("Gender:" + Gender + "   ");
            s.Append("ClassFilter:" + ClassFilter + "   ");
            s.AppendLine("DisplayTimeAdjustment:" + DisplayTimeAdjustment);
            s.Append(' ', pad); s.AppendLine("LineSpeaker:" + LineSpeaker);
            s.Append(' ', pad); s.AppendLine("SpeakerTarget:" + SpeakerTarget);
            //s.Append(' ', pad); s.AppendLine("Comment:" + Comment);

            s.Append(' ', pad); s.AppendLine("ConvLocalDisplayTimes: not shown");
            //for (int i = 0; i < ConvLocalDisplayTimes.Length; i++)
            //    ConvLocalDisplayTimes[i].AsText(s, pad);

            if (TrueNodes.Count > 0)
            {
                s.Append(' ', pad); s.AppendLine("TrueNodes:");
                s.Append(' ', pad); s.AppendLine("{");
                foreach (var node in TrueNodes)
                    node.AsText(s, pad + 3);
                s.Append(' ', pad); s.AppendLine("}");
            }
            if (FalseNodes.Count > 0)
            {
                s.Append(' ', pad); s.AppendLine("FalseNodes:");
                s.Append(' ', pad); s.AppendLine("{");
                foreach (var node in FalseNodes)
                    node.AsText(s, pad + 3);
                s.Append(' ', pad); s.AppendLine("}");
            }
            if (ChildNodes.Count > 0)
            {
                s.Append(' ', pad); s.AppendLine("ChildNodes:");
                s.Append(' ', pad); s.AppendLine("{");
                foreach (var node in ChildNodes)
                    node.AsText(s, pad + 3);
                s.Append(' ', pad); s.AppendLine("}");
            }
        }
    }

    public class ConvLocalDisplayTimes
    {
        public int[] Languages = new int[14];

        public ConvLocalDisplayTimes(MpqFileStream stream)
        {
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = stream.ReadValueS32();
        }

        public void AsText(StringBuilder s, int pad)
        {
            s.Append(' ', pad);
            for (int i = 0; i < Languages.Length; i++)
                s.Append(Languages[i] + "  ");
            s.AppendLine();
        }
    }


    public enum ConversationTypes
    {
        FollowerSoundset = 0,
        PlayerEmote = 1,
        AmbientFloat = 2,
        FollowerBanter = 3,
        FollowerCallout = 4,
        PlayerCallout = 5,
        GlobalChatter = 6,
        GlobalFloat = 7,
        LoreBook = 8,
        AmbientGossip = 9,
        TalkMenuGossip = 10,
        QuestStandard = 11,
        QuestFloat = 12,
        QuestEvent = 13
    }


    public enum Speaker
    {
        None = -1,
        Player = 0,
        PrimaryNPC = 1,
        AltNPC1 = 2,
        AltNPC2 = 3,
        AltNPC3 = 4,
        AltNPC4 = 5,
        TemplarFollower = 6,
        ScoundrelFollower = 7,
        EnchantressFollower = 8
    }

    public enum TypeConv
    {
        None = -1,
        EqualTo = 0,
        LessThan = 1,
        GreaterThan = 2,
        LessThanOrEqualTo = 3,
        GreaterThanOrEqualTo = 4,
        NotEqualTo = 5
    }

    public enum Ref
    {
        None = -1,
        IncrementedBy = 0,
        DecrementedBy = 1,
        SetTo = 2
    }
}

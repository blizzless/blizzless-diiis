//Blizzless Project 2022
//Blizzless Project 2022 
using CrystalMpq;
//Blizzless Project 2022 
using DiIiS_NA.Core.MPQ.FileFormats.Types;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.Math;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.Core.Types.SNO;
//Blizzless Project 2022 
using Gibbed.IO;
//Blizzless Project 2022 
using System.Collections.Generic;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.Act)]
    public class Act : FileFormat
    {
        public Header Header { get; private set; }
        public List<ActQuestInfo> ActQuestInfo { get; private set; }
        public WaypointInfo[] WayPointInfo { get; private set; }
        public ResolvedPortalDestination ResolvedPortalDestination { get; private set; }
        public ActStartLocOverride[] ActStartLocOverrides { get; private set; }

        public Act(MpqFile file)
        {
            var stream = file.Open();
            this.Header = new Header(stream);

            this.ActQuestInfo = stream.ReadSerializedData<ActQuestInfo>(); //12
            stream.Position += 12;

            this.WayPointInfo = new WaypointInfo[100]; //32
            for (int i = 0; i < WayPointInfo.Length; i++)
                this.WayPointInfo[i] = new WaypointInfo(stream);

            this.ResolvedPortalDestination = new ResolvedPortalDestination(stream);

            this.ActStartLocOverrides = new ActStartLocOverride[6];
            for (int i = 0; i < ActStartLocOverrides.Length; i++)
                this.ActStartLocOverrides[i] = new ActStartLocOverride(stream);

            stream.Close();
        }
    }

    public class WaypointInfo
    {
        public int Flags { get; private set; }
        public int SNOWorld { get; private set; }
        public int SNOLevelArea { get; private set; }
        public int WaypointButtonNormal { get; private set; }
        public int WaypointButtonMouseOver { get; private set; }
        public int WaypointButtonPushed { get; private set; }
        public int SNOQuestRange { get; private set; }
        public int ZoneLabel { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public Vector2D Coords { get; private set; }

        public WaypointInfo(MpqFileStream stream)
        {
            Flags = stream.ReadValueS32();
            SNOWorld = stream.ReadValueS32();
            SNOLevelArea = stream.ReadValueS32();
            WaypointButtonNormal = stream.ReadValueS32();
            WaypointButtonMouseOver = stream.ReadValueS32();
            WaypointButtonPushed = stream.ReadValueS32();
            SNOQuestRange = stream.ReadValueS32();
            ZoneLabel = stream.ReadValueS32();
            Coords = new Vector2D(stream);
            //X = stream.ReadValueS32();
            //Y = stream.ReadValueS32();
        }
    }

    public class ActStartLocOverride
    {
        public ResolvedPortalDestination ResolvedPortalDestination { get; private set; }
        public int SNOQuestRange { get; private set; }
        public int HubWorldsSNO { get; private set; }
        public int DisallowReturnToTown { get; private set; }

        public ActStartLocOverride(MpqFileStream stream)
        {
            ResolvedPortalDestination = new ResolvedPortalDestination(stream);
            SNOQuestRange = stream.ReadValueS32();
            HubWorldsSNO = stream.ReadValueS32();
            DisallowReturnToTown = stream.ReadValueS32();
        }
    }


    public class ResolvedPortalDestination
    {
        public int SNOWorld { get; private set; }
        public int EntranceGUID { get; private set; }
        public int SNODestLevelArea { get; private set; }

        public ResolvedPortalDestination(MpqFileStream stream)
        {
            SNOWorld = stream.ReadValueS32();
            EntranceGUID = stream.ReadValueS32();
            SNODestLevelArea = stream.ReadValueS32();
        }
    }

    public class ActQuestInfo : ISerializableData
    {
        public int SNOQuest { get; private set; }

        public void Read(MpqFileStream stream)
        {
            SNOQuest = stream.ReadValueS32();
        }
    }
}

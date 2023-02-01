//Blizzless Project 2022
using System.Collections.Generic;
using CrystalMpq;
using Gibbed.IO;
using DiIiS_NA.GameServer.Core.Types.SNO;
using DiIiS_NA.Core.MPQ.FileFormats.Types;
using DiIiS_NA.Core.Storage;

namespace DiIiS_NA.Core.MPQ.FileFormats
{
    [FileFormat(SNOGroup.ConversationList)]
    public class ConversationList : FileFormat
    {
        [PersistentProperty("ConversationList")]
        public List<ConversationListEntry> ConversationListEntries { get; set; }
        public List<ConversationListEntry> AmbientConversationListEntries { get; set; }

        public ConversationList() 
        {
            if (ConversationListEntries == null) ConversationListEntries = new List<ConversationListEntry>();
            if (AmbientConversationListEntries == null) AmbientConversationListEntries = new List<ConversationListEntry>();
        }
    }


    public class ConversationListEntry
    { 
        public ConversationTypes Type
        {
            get
            {
                return (ConversationTypes)Flags;
            }
            set
            {
                Flags = (int)value;
            }
        }

        [PersistentProperty("SNOConversation")]
        public int SNOConversation { get; private set; }

        [PersistentProperty("I0")]
        public int ConditionReqs { get; private set; }

        [PersistentProperty("I1")]
        public int Flags { get; private set; }

        [PersistentProperty("I2")]
        public int CrafterType { get; private set; }

        [PersistentProperty("GbidItem")]
        public int GbidItem { get; private set; }

        [PersistentProperty("Noname1")]
        public string Label { get; private set; }

        [PersistentProperty("Noname2")]
        public string PlayerFlag { get; private set; }

        [PersistentProperty("SNOQuestCurrent")]
        public int SNOQuestCurrent { get; private set; }

        [PersistentProperty("I3")]
        public int StepUIDCurrent { get; private set; }

        [PersistentProperty("Act")]
        public int SpecialEventFlag { get; private set; }
        
        [PersistentProperty("SNOQuestAssigned")]
        public int SNOQuestAssigned { get; private set; }

        [PersistentProperty("SNOQuestActive")]
        public int SNOQuestActive { get; private set; }

        [PersistentProperty("SNOQuestComplete")]
        public int SNOQuestComplete { get; private set; }

        [PersistentProperty("SNOQuestRange")]
        public int SNOQuestRange { get; private set; }

        [PersistentProperty("SNOLevelArea")]
        public int SNOLevelArea { get; private set; }

        public ConversationListEntry() { }
        //*
        public ConversationListEntry(ConversationTypes type, int i0, int questId, int convId, int questStep, int act)
        {
            SNOConversation = convId;
            SpecialEventFlag = act;
            Type = type;
            SNOQuestCurrent = -1;
            SNOQuestAssigned = -1;
            SNOQuestComplete = -1;
            SNOQuestRange = -1;
            SNOLevelArea = -1;
            SNOQuestActive = questId;
            ConditionReqs = i0;
            //this.I1 = -1;
            CrafterType = -1;
            StepUIDCurrent = questStep;
            GbidItem = -1;
            Label = "";
            PlayerFlag = "";
        }
    }
}

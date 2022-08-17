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

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(Opcodes.QuestUpdateMessage)]
    public class QuestUpdateMessage : GameMessage
    {
        public int snoQuest;
        public int snoLevelArea;
        public int StepID;        
        public bool DisplayButton;   
        public bool Failed;       

        public QuestUpdateMessage() : base(Opcodes.QuestUpdateMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoQuest = buffer.ReadInt(32);
            snoLevelArea = buffer.ReadInt(32);
            StepID = buffer.ReadInt(32);
            DisplayButton = buffer.ReadBool();
            Failed = buffer.ReadBool();
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoQuest);
            buffer.WriteInt(32, snoLevelArea);
            buffer.WriteInt(32, StepID);
            buffer.WriteBool(DisplayButton);
            buffer.WriteBool(Failed);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("QuestUpdateMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoQuest: 0x" + snoQuest.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("snoLevelArea: 0x" + snoLevelArea.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("StepID: 0x" + StepID.ToString("X8") + " (" + StepID + ")");
            b.Append(' ', pad); b.AppendLine("DisplayButton: " + (DisplayButton ? "true" : "false"));
            b.Append(' ', pad); b.AppendLine("Failed: " + (Failed ? "true" : "false"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

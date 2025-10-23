using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(Opcodes.QuestCounterMessage)]
    public class QuestCounterMessage : GameMessage
    {
        public int snoQuest;
        public int snoLevelArea;
        public int StepID;          // The logical sequence of steps in a quest can be an arbitrary sequence of ids 
        public int TaskIndex;       // 0-bound index of the task to update.
        public int Counter;         // Value of the counter of the task. Used for tasks like "1 of 4 monsters slain"
        public int Checked;         // 0 = Task is unchecked, 1 = Task is checked (completed)

        public QuestCounterMessage() : base(Opcodes.QuestCounterMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            snoQuest = buffer.ReadInt(32);
            snoLevelArea = buffer.ReadInt(32);
            StepID = buffer.ReadInt(32);
            TaskIndex = buffer.ReadInt(32);
            Counter = buffer.ReadInt(32);
            Checked = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, snoQuest);
            buffer.WriteInt(32, snoLevelArea);
            buffer.WriteInt(32, StepID);
            buffer.WriteInt(32, TaskIndex);
            buffer.WriteInt(32, Counter);
            buffer.WriteInt(32, Checked);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("QuestCounterMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("snoQuest: 0x" + snoQuest.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("snoLevelArea: 0x" + snoLevelArea.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("StepID: 0x" + StepID.ToString("X8") + " (" + StepID + ")");
            b.Append(' ', pad); b.AppendLine("TaskIndex: 0x" + TaskIndex.ToString("X8") + " (" + TaskIndex + ")");
            b.Append(' ', pad); b.AppendLine("Counter: 0x" + Counter.ToString("X8") + " (" + Counter + ")");
            b.Append(' ', pad); b.AppendLine("Checked: 0x" + Checked.ToString("X8") + " (" + Checked + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}

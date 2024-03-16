using DiIiS_NA.GameServer.Core.Types.SNO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.MessageSystem.Message.Definitions.Quest
{
    [Message(Opcodes.DynamicQuestObjectiveMessage)]
    public class DynamicQuestObjectiveMessage : GameMessage
    {
        public int UID;
        public int eObjectType;
        public int Param1;
        public int Param2;
        public SNOHandle SnoName1;
        public SNOHandle SnoName2;
        public int /* gbid */ GBIDParam1;
        public int /* gbid */ GBIDParam2;
        public int HiddenUntilComplete;
        public int Flags;
        public int /* gbid */ GBIDItemtoShow;
        public int State;
        public int Completed;
        public int Hidden;

        public DynamicQuestObjectiveMessage() : base(Opcodes.DynamicQuestObjectiveMessage) { }

        public override void Parse(GameBitBuffer buffer)
        {
            UID = buffer.ReadInt(32);
            eObjectType = buffer.ReadInt(32);
            Param1 = buffer.ReadInt(32);
            Param2 = buffer.ReadInt(32);
            SnoName1.Parse(buffer);
            SnoName2.Parse(buffer);
            GBIDParam1 = buffer.ReadInt(32);
            GBIDParam2 = buffer.ReadInt(32);
            HiddenUntilComplete = buffer.ReadInt(32);
            Flags = buffer.ReadInt(32);
            GBIDItemtoShow = buffer.ReadInt(32);
            State = buffer.ReadInt(32);
            Completed = buffer.ReadInt(32);
            Hidden = buffer.ReadInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, UID);
            buffer.WriteInt(32, eObjectType);
            buffer.WriteInt(32, Param1);
            buffer.WriteInt(32, Param2);
            SnoName1.Encode(buffer);
            SnoName2.Encode(buffer);
            buffer.WriteInt(32, GBIDParam1);
            buffer.WriteInt(32, GBIDParam2);
            buffer.WriteInt(32, HiddenUntilComplete);
            buffer.WriteInt(32, Flags);
            buffer.WriteInt(32, GBIDItemtoShow);
            buffer.WriteInt(32, State);
            buffer.WriteInt(32, Completed);
            buffer.WriteInt(32, Hidden);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            throw new NotImplementedException();
        }
    }
}

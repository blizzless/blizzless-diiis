//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.whisper.v1;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.ServicesSystem;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
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

namespace DiIiS_NA.BGS_Server.ServicesSystem.Services
{
    [Service(serviceID: 0x96, serviceHash: 0xC12828F9)]
    public class WhisperService : bgs.protocol.whisper.v1.WhisperService, IServerService
    {
        public override void AdvanceClearTime(IRpcController controller, AdvanceClearTimeRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void AdvanceViewTime(IRpcController controller, AdvanceViewTimeRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void GetWhisperMessages(IRpcController controller, GetWhisperMessagesRequest request, Action<GetWhisperMessagesResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void SendWhisper(IRpcController controller, SendWhisperRequest request, Action<SendWhisperResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void SetTypingIndicator(IRpcController controller, SetTypingIndicatorRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<SubscribeResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
    }
}

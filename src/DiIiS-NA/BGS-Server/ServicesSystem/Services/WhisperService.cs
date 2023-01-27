//Blizzless Project 2022
using bgs.protocol;
using bgs.protocol.whisper.v1;
using DiIiS_NA.LoginServer.ServicesSystem;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

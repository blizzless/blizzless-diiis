//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.channel.v2.membership;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x26, serviceHash: 2119327385)]
    public class ChannelMembershipService_ : bgs.protocol.channel.v2.membership.ChannelMembershipService, IServerService
    {
        public override void GetState(IRpcController controller, GetStateRequest request, Action<GetStateResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<SubscribeResponse> done)
        {
            done(SubscribeResponse.CreateBuilder().Build());
        }

        public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
    }
}

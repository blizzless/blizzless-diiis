//Blizzless Project 2022
using bgs.protocol;
using bgs.protocol.sns.v1;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x51, serviceName: "bnet.protocol.sns.SocialNetworkService")]

    public class SocialNetworkService : bgs.protocol.sns.v1.SocialNetworkService, IServerService
    {
        public override void GetFacebookAccountLinkStatus(IRpcController controller, GetFacebookAccountLinkStatusRequest request, Action<GetFacebookAccountLinkStatusResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetFacebookAuthCode(IRpcController controller, GetFacebookAuthCodeRequest request, Action<GetFacebookAuthCodeResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetFacebookBnetFriends(IRpcController controller, GetFacebookBnetFriendsRequest request, Action<NoData> done)
        {
            done(NoData.CreateBuilder().Build());
        }

        public override void GetFacebookSettings(IRpcController controller, NoData request, Action<GetFacebookSettingsResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGoogleAccountLinkStatus(IRpcController controller, GetGoogleAccountLinkStatusRequest request, Action<GetGoogleAccountLinkStatusResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGoogleAuthToken(IRpcController controller, GetGoogleAuthTokenRequest request, Action<GetGoogleAuthTokenResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetGoogleSettings(IRpcController controller, NoData request, Action<GetGoogleSettingsResponse> done)
        {
            throw new NotImplementedException();
        }
    }
}

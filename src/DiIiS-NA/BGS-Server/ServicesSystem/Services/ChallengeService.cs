using bgs.protocol;
using bgs.protocol.challenge.v1;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x78, serviceHash: 0xBBDA171F)]
    public class ChallengeService : bgs.protocol.challenge.v1.ChallengeService, IServerService
    {
        public override void ChallengeAnswered(IRpcController controller, ChallengeAnsweredRequest request, Action<ChallengeAnsweredResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void ChallengeCancelled(IRpcController controller, ChallengeCancelledRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void ChallengePicked(IRpcController controller, ChallengePickedRequest request, Action<ChallengePickedResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void SendChallengeToUser(IRpcController controller, SendChallengeToUserRequest request, Action<SendChallengeToUserResponse> done)
        {
            throw new NotImplementedException();
        }
    }
}

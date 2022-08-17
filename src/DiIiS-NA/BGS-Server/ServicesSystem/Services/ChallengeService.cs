//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.challenge.v1;
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

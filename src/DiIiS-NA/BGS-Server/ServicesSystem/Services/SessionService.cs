//Blizzless Project 2022
using System;
using bgs.protocol;
using bgs.protocol.session.v1;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.LoginServer.Base;
using Google.ProtocolBuffers;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x13, serviceName: "bnet.protocol.session.SessionService")]
    public class SessionService : bgs.protocol.session.v1.SessionService, IServerService
    {
        public override void CreateSession(IRpcController controller, CreateSessionRequest request, Action<CreateSessionResponse> done)
        {
            string Start = "A7B5C8B0593FFEC10000000";
            string End = "BCABD";

            string session = Start + RandomHelper.Next(0, 9).ToString() + RandomHelper.Next(0, 9).ToString() + RandomHelper.Next(0, 9).ToString() + RandomHelper.Next(0, 9).ToString() + End;
            CreateSessionResponse.Builder builder = CreateSessionResponse.CreateBuilder();
            builder.SetSessionId(session);
            done(builder.Build());

            SessionCreatedNotification.Builder n = SessionCreatedNotification.CreateBuilder();
            n.SetIdentity(request.Identity)
             .SetReason(0)
             .SetSessionId(session);
            (controller as HandlerController).Client.MakeRPC((lid) => SessionListener.CreateStub((controller as HandlerController).Client).OnSessionCreated(controller, n.Build(), callback => { }));
        }

        private void DisconnectClient(HandlerController controller)
        {
            if (controller.Client.Account != null && controller.Client.Account.GameAccount != null) controller.Client.Account.GameAccount.LoggedInClient = null;
            LoginServer.Battle.PlayerManager.PlayerDisconnected(controller.Client);
        }

        public override void DestroySession(IRpcController controller, DestroySessionRequest request, Action<NoData> done)
        {
            Console.WriteLine("Клиент - {0} , отключен", (controller as HandlerController).Client.SocketConnection.RemoteAddress);
            this.DisconnectClient(controller as HandlerController);
            if ((controller as HandlerController).Client.Account != null)
                (controller as HandlerController).Client.Account.GameAccount.Logined = false;
            ((controller as HandlerController).Client).Connect.CloseAsync();
            (controller as HandlerController).Client.SocketConnection.CloseAsync();

            done(NoData.CreateBuilder().Build());
        }


        public override void GetSessionState(IRpcController controller, GetSessionStateRequest request, Action<GetSessionStateResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetSessionStateByBenefactor(IRpcController controller, GetSessionStateByBenefactorRequest request, Action<GetSessionStateByBenefactorResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void GetSignedSessionState(IRpcController controller, GetSignedSessionStateRequest request, Action<GetSignedSessionStateResponse> done)
        {
            done(GetSignedSessionStateResponse.CreateBuilder().SetToken("eyJ0eXAiOiJKV1QiLCJlbnYiOiJwcm9kLmV1IiwiYWxnIjoiUlMyNTYiLCJraWQiOiJmMDE5NzgzMi0zMWMwLTQzN2MtOTc2NC1iMzliOTM5MDJlNWMiLCJrdHkiOiJSU0EifQ").Build());
        }

        public override void MarkSessionsAlive(IRpcController controller, MarkSessionsAliveRequest request, Action<MarkSessionsAliveResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void RefreshSessionKey(IRpcController controller, RefreshSessionKeyRequest request, Action<RefreshSessionKeyResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void UpdateSession(IRpcController controller, UpdateSessionRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
    }
}

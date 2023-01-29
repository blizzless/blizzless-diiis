//Blizzless Project 2022
using System;
using System.Reflection;
using bgs.protocol;
using bgs.protocol.session.v1;
using DiIiS_NA.Core.Helpers.Math;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Base;
using Google.ProtocolBuffers;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x13, serviceName: "bnet.protocol.session.SessionService")]
    public class SessionService : bgs.protocol.session.v1.SessionService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger(nameof(SessionService)); 
        public override void CreateSession(IRpcController controller, CreateSessionRequest request, Action<CreateSessionResponse> done)
        {
            string Start = "A7B5C8B0593FFEC10000000";
            string End = "BCABD";

            string session = Start + RandomHelper.Next(0, 9) + RandomHelper.Next(0, 9) + RandomHelper.Next(0, 9) + RandomHelper.Next(0, 9) + End;
            CreateSessionResponse.Builder builder = CreateSessionResponse.CreateBuilder();
            builder.SetSessionId(session);
            done(builder.Build());

            SessionCreatedNotification.Builder n = SessionCreatedNotification.CreateBuilder();
            n.SetIdentity(request.Identity)
             .SetReason(0)
             .SetSessionId(session);
            ((HandlerController) controller).Client.MakeRpc((lid) => SessionListener.CreateStub(((HandlerController) controller).Client).OnSessionCreated(controller, n.Build(), callback => { }));
        }

        private void DisconnectClient(HandlerController controller)
        {
            if (controller.Client.Account is { GameAccount: { } }) controller.Client.Account.GameAccount.LoggedInClient = null;
            Battle.PlayerManager.PlayerDisconnected(controller.Client);
        }

        public override void DestroySession(IRpcController controller, DestroySessionRequest request, Action<NoData> done)
        {
            Logger.MethodTrace("");
            Logger.Trace("Destroying game session for client {0}", ((HandlerController) controller).Client);
            if (controller is HandlerController handlerController)
            {
                DisconnectClient(handlerController);
                if (handlerController.Client.Account != null)
                    handlerController.Client.Account.GameAccount.IsLoggedIn = false;
                (handlerController.Client).Connect.CloseAsync();
                handlerController.Client.SocketConnection.CloseAsync();
            }

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
            => done(GetSignedSessionStateResponse.CreateBuilder().SetToken("eyJ0eXAiOiJKV1QiLCJlbnYiOiJwcm9kLmV1IiwiYWxnIjoiUlMyNTYiLCJraWQiOiJmMDE5NzgzMi0zMWMwLTQzN2MtOTc2NC1iMzliOTM5MDJlNWMiLCJrdHkiOiJSU0EifQ").Build());

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

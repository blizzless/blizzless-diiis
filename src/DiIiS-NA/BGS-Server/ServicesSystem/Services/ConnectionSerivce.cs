using DiIiS_NA.LoginServer.Base;
using System;
using bgs.protocol;
using bgs.protocol.authentication.v1;
using bgs.protocol.connection.v1;
using Google.ProtocolBuffers;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Extensions;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x0, serviceHash: 1698982289)]
    public class ConnectionSvc : ConnectionService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        public override void Bind(IRpcController controller, BindRequest request, Action<BindResponse> done)
        {
            throw new NotImplementedException();
        }

        public override void Connect(IRpcController controller, ConnectRequest request, Action<ConnectResponse> done)
        {
            var builder = ConnectResponse.CreateBuilder()
                .SetServerId(ProcessId.CreateBuilder().SetLabel(0).SetEpoch(DateTime.Now.ToUnixTime()))
                .SetServerTime(DateTime.Now.ToUnixTime())
                .SetClientId(ProcessId.CreateBuilder().SetLabel(1).SetEpoch(DateTime.Now.ToUnixTime()));
            if (request.HasUseBindlessRpc)
                builder.SetUseBindlessRpc(true);
            
            ((HandlerController) controller).Client.Services.Add(0x54DFDA17, 0x01);
            ((HandlerController) controller).Client.Services.Add(0xD4DCD093, 0x02);
            ((HandlerController) controller).Client.Services.Add(0x71240E35, 0x03);
            ((HandlerController) controller).Client.Services.Add(0xBBDA171F, 0x04);
            ((HandlerController) controller).Client.Services.Add(0xF084FC20, 0x05);
            ((HandlerController) controller).Client.Services.Add(0xBF8C8094, 0x06);
            ((HandlerController) controller).Client.Services.Add(0x166FE4A1, 0x07);
            ((HandlerController) controller).Client.Services.Add(0xB96F5297, 0x08);
            ((HandlerController) controller).Client.Services.Add(0x6F259A13, 0x09);
            ((HandlerController) controller).Client.Services.Add(0xE1CB2EA8, 0x0A);
            ((HandlerController) controller).Client.Services.Add(0xBC872C22, 0x0B);
            ((HandlerController) controller).Client.Services.Add(0x7FE36B32, 0x0C);
            ((HandlerController) controller).Client.Services.Add(233634817, 0x0D);
            ((HandlerController) controller).Client.Services.Add(0x62DA0891, 0x0E); //AccountService
            ((HandlerController) controller).Client.Services.Add(510168069, 0x0F);
            ((HandlerController) controller).Client.Services.Add(0x45E59C4D, 0x10);
            ((HandlerController) controller).Client.Services.Add(0x135185EF, 0x11);
            ((HandlerController) controller).Client.Services.Add(1910276758, 0x51);
            //((HandlerController) controller).Client.Services.Add(2495170438, 0x25);
            ((HandlerController) controller).Client.Services.Add(2119327385, 0x26);
            
            done(builder.Build());
            Logger.Info("Connect with Blizzless established. Client - {0}", ((HandlerController) controller).Client.SocketConnection.RemoteAddress);
        }

        public override void Echo(IRpcController controller, EchoRequest request, Action<EchoResponse> done)
        {
            throw new NotImplementedException();
        }
        public override void Encrypt(IRpcController controller, EncryptRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
        public override void ForceDisconnect(IRpcController controller, DisconnectNotification request, Action<NO_RESPONSE> done)
        {
            throw new NotImplementedException();
        }
        public override void KeepAlive(IRpcController controller, NoData request, Action<NO_RESPONSE> done)
        {
            var builder = LogonUpdateRequest.CreateBuilder().SetErrorCode(0);

            done(NO_RESPONSE.CreateBuilder().Build());
        }

        public override void RequestDisconnect(IRpcController controller, DisconnectRequest request, Action<NO_RESPONSE> done)
        {
            Logger.Info("Client - {0} , disconnected", ((HandlerController) controller).Client.SocketConnection.RemoteAddress);
            DisconnectClient((HandlerController) controller);
            if (((HandlerController) controller).Client.Account != null)
                ((HandlerController) controller).Client.Account.GameAccount.IsLoggedIn = false;
            (((HandlerController) controller).Client).Connect.CloseAsync();
            ((HandlerController) controller).Client.SocketConnection.CloseAsync();
            /*
            if (((HandlerController) controller).Client.Account != null)
            {
                ((HandlerController) controller).Client.Account.CurrentGameAccount.Logined = false;
                AccountManager.SaveToDB(((HandlerController) controller).Client.Account);
                if (((HandlerController) controller).Client.Account.CurrentGameAccount != null)
                {
                    GameAccountManager.SaveToDB(((HandlerController) controller).Client.Account.CurrentGameAccount);
                    ((HandlerController) controller).Client.SocketConnection.CloseAsync();
                    ((HandlerController) controller).Client.Connect.CloseAsync();
                }
            }
            //*/

        }
        private void DisconnectClient(HandlerController controller)
        {
            if (controller.Client.Account != null && controller.Client.Account.GameAccount != null) controller.Client.Account.GameAccount.LoggedInClient = null;
            Battle.PlayerManager.PlayerDisconnected(controller.Client);
        }
    }
}

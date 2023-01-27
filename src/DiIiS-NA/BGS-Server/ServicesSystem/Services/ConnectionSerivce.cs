//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.authentication.v1;
//Blizzless Project 2022 
using bgs.protocol.connection.v1;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
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
            
            (controller as HandlerController).Client.Services.Add(0x54DFDA17, 0x01);
            (controller as HandlerController).Client.Services.Add(0xD4DCD093, 0x02);
            (controller as HandlerController).Client.Services.Add(0x71240E35, 0x03);
            (controller as HandlerController).Client.Services.Add(0xBBDA171F, 0x04);
            (controller as HandlerController).Client.Services.Add(0xF084FC20, 0x05);
            (controller as HandlerController).Client.Services.Add(0xBF8C8094, 0x06);
            (controller as HandlerController).Client.Services.Add(0x166FE4A1, 0x07);
            (controller as HandlerController).Client.Services.Add(0xB96F5297, 0x08);
            (controller as HandlerController).Client.Services.Add(0x6F259A13, 0x09);
            (controller as HandlerController).Client.Services.Add(0xE1CB2EA8, 0x0A);
            (controller as HandlerController).Client.Services.Add(0xBC872C22, 0x0B);
            (controller as HandlerController).Client.Services.Add(0x7FE36B32, 0x0C);
            (controller as HandlerController).Client.Services.Add(233634817, 0x0D);
            (controller as HandlerController).Client.Services.Add(0x62DA0891, 0x0E); //AccountService
            (controller as HandlerController).Client.Services.Add(510168069, 0x0F);
            (controller as HandlerController).Client.Services.Add(0x45E59C4D, 0x10);
            (controller as HandlerController).Client.Services.Add(0x135185EF, 0x11);
            (controller as HandlerController).Client.Services.Add(1910276758, 0x51);
            //(controller as HandlerController).Client.Services.Add(2495170438, 0x25);
            (controller as HandlerController).Client.Services.Add(2119327385, 0x26);
            
            done(builder.Build());
            Logger.Info("Connect with $[dodgerblue1]$Blizz$[/]$$[deepskyblue2]$less$[/]$ established. Client - {0}", (controller as HandlerController).Client.SocketConnection.RemoteAddress);
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
            Logger.Info("Client - {0} , disconnected", (controller as HandlerController).Client.SocketConnection.RemoteAddress);
            this.DisconnectClient(controller as HandlerController);
            if ((controller as HandlerController).Client.Account != null)
                (controller as HandlerController).Client.Account.GameAccount.Logined = false;
            ((controller as HandlerController).Client).Connect.CloseAsync();
            (controller as HandlerController).Client.SocketConnection.CloseAsync();
            /*
            if ((controller as HandlerController).Client.Account != null)
            {
                (controller as HandlerController).Client.Account.CurrentGameAccount.Logined = false;
                AccountManager.SaveToDB((controller as HandlerController).Client.Account);
                if ((controller as HandlerController).Client.Account.CurrentGameAccount != null)
                {
                    GameAccountManager.SaveToDB((controller as HandlerController).Client.Account.CurrentGameAccount);
                    (controller as HandlerController).Client.SocketConnection.CloseAsync();
                    (controller as HandlerController).Client.Connect.CloseAsync();
                }
            }
            //*/

        }
        private void DisconnectClient(HandlerController controller)
        {
            if (controller.Client.Account != null && controller.Client.Account.GameAccount != null) controller.Client.Account.GameAccount.LoggedInClient = null;
            LoginServer.Battle.PlayerManager.PlayerDisconnected(controller.Client);
        }
    }
}

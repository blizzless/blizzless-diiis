//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.notification.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.CommandManager;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using Google.ProtocolBuffers;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0xa, serviceName: "bnet.protocol.notification.NotificationService")]
    public class NotificationService : bgs.protocol.notification.v1.NotificationService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public override void Publish(IRpcController controller, PublishRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void SendNotification(IRpcController controller, Notification request, Action<NoData> done)
        {

            switch (request.GetNotificationType())
            {
                case NotificationTypeHelper.NotificationType.Whisper:

                    var targetAccount = GameAccountManager.GetAccountByPersistentID(request.TargetId.Low);
                    Logger.Trace(string.Format("NotificationRequest.Whisper by {0} to {1}", (controller as HandlerController).Client.Account.GameAccount, targetAccount));

                    if (targetAccount.LoggedInClient == null) return;

                    if (targetAccount == (controller as HandlerController).Client.Account.GameAccount)
                        CommandManager.TryParse(request.AttributeList[0].Value.StringValue, (controller as HandlerController).Client); // try parsing it as a command and respond it if so.
                    else
                    {
                        var notification = bgs.protocol.notification.v1.Notification.CreateBuilder(request)
                            .SetSenderId((controller as HandlerController).Client.Account.GameAccount.BnetEntityId)
                            .SetSenderAccountId((controller as HandlerController).Client.Account.BnetEntityId)
                            .Build();

                        targetAccount.LoggedInClient.MakeRPC((lid) =>
                            NotificationListener.CreateStub(targetAccount.LoggedInClient).OnNotificationReceived(controller, notification, callback => { }));
                    }
                    break;
                default:
                    Logger.Warn("Unhandled notification type: {0}", request.Type);
                    break;
            }
            //*/
            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<NoData> done)
        {
            //throw new NotImplementedException();
            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
        {
            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());
            //throw new NotImplementedException();
        }
    }
}

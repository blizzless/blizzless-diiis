//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.matchmaking.v1;
//Blizzless Project 2022 
using D3.OnlineService;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.GamesSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x57, serviceName: "bnet.protocol.matchmaking.GameRequest")]
    public class GameRequestService : bgs.protocol.matchmaking.v1.GameRequestService, IServerService
    {
        public static ulong Counter = 1;
        public override void CancelMatchmaking(IRpcController controller, CancelMatchmakingRequest request, Action<NoData> done)
        {
            done(NoData.DefaultInstance);
        }
        public override void JoinGame(IRpcController controller, JoinGameRequest request, Action<JoinGameResponse> done)
        {
            throw new NotImplementedException();
        }
        public override void QueueMatchmaking(IRpcController controller, QueueMatchmakingRequest request, Action<QueueMatchmakingResponse> done)
        {
            #region Инициализация игры

            var id = RequestId.CreateBuilder().SetId(Counter); Counter++;

            done(QueueMatchmakingResponse.CreateBuilder().SetRequestId(id).Build());
            #endregion
            string request_type = "";
            string ServerPool = "";
            bgs.protocol.v2.Attribute AttributeOfServer = null;
            GameCreateParams gameCreateParams = null;

            int Difficulty = 0;
            int CurrentAct = 0;
            int CurrentQuest = 0;
            int CurrentStep = 0;
            string GameTag = "";
            foreach (var attr in request.Options.CreationProperties.AttributeList)
            {
                switch (attr.Name)
                {
                    case "GameCreateParams":
                        gameCreateParams = GameCreateParams.ParseFrom(attr.Value.BlobValue);
                        AttributeOfServer = attr;
                        break;
                    case "ServerPool":
                        ServerPool = attr.Value.StringValue;
                        break;
                    case "request_type":
                        request_type = attr.Value.StringValue;
                        break;
                }
            }

            Difficulty = gameCreateParams.CampaignOrAdventureMode.HandicapLevel;
            CurrentAct = gameCreateParams.CampaignOrAdventureMode.Act;
            CurrentQuest = gameCreateParams.CampaignOrAdventureMode.SnoQuest;
            CurrentStep = gameCreateParams.CampaignOrAdventureMode.QuestStepId;

            #region Ставим в очередь
            QueueWaitTimes.Builder timers = QueueWaitTimes.CreateBuilder();
            timers.SetMinWait(0).SetMaxWait(120).SetAvgWait(60).SetStdDevWait(0);

            var member = bgs.protocol.account.v1.GameAccountHandle.CreateBuilder();
            member.SetId((uint)(controller as HandlerController).Client.Account.GameAccount.BnetEntityId.Low).SetProgram(0x00004433).SetRegion(1);

            QueueEntryNotification.Builder qen = QueueEntryNotification.CreateBuilder();
            qen.SetRequestId(id).SetWaitTimes(timers).AddMember(member).SetRequestInitiator(member);
            (controller as HandlerController).Client.MakeRPC((lid) => GameRequestListener.CreateStub((controller as HandlerController).Client).OnQueueEntry(new HandlerController() { ListenerId = lid }, qen.Build(), callback => { }));
            #endregion

            #region Обновление очереди
            QueueUpdateNotification.Builder qun = QueueUpdateNotification.CreateBuilder();
            qun.SetRequestId(id)
                .SetWaitTimes(timers)
                .SetIsMatchmaking(true);
            (controller as HandlerController).Client.MakeRPC((lid) => GameRequestListener.CreateStub((controller as HandlerController).Client).OnQueueUpdate(new HandlerController() { ListenerId = lid }, qun.Build(), callback => { }));
            #endregion


            string GAME_SERVER_IP = Program.GAMESERVERIP;
            if (GameServer.NATConfig.Instance.Enabled)
                GAME_SERVER_IP = Program.PUBLICGAMESERVERIP;
            uint GAME_SERVER_PORT = 2001;

            MatchmakingResultNotification.Builder notification = MatchmakingResultNotification.CreateBuilder();
            ConnectInfo.Builder connectInfo = ConnectInfo.CreateBuilder();
            connectInfo.SetAddress(Address.CreateBuilder().SetAddress_(GAME_SERVER_IP).SetPort(GAME_SERVER_PORT));
            connectInfo.AddAttribute(bgs.protocol.v2.Attribute.CreateBuilder().SetName("GameAccount").SetValue(bgs.protocol.v2.Variant.CreateBuilder().SetBlobValue(member.Build().ToByteString())));
            connectInfo.AddAttribute(bgs.protocol.v2.Attribute.CreateBuilder().SetName("Token").SetValue(bgs.protocol.v2.Variant.CreateBuilder().SetUintValue(0xEEF4364684EE186E))); // FIXME
            connectInfo.AddAttribute(AttributeOfServer); // Настройки игры

            GameHandle.Builder gh = GameHandle.CreateBuilder();
            gh.SetMatchmaker(MatchmakerHandle.CreateBuilder()
                                .SetId((uint)(controller as HandlerController).Client.Account.GameAccount.BnetEntityId.Low)
                                .SetAddr(HostProxyPair.CreateBuilder()
                                            .SetHost(ProcessId.CreateBuilder().SetLabel(1250).SetEpoch(1499729350))
                                            .SetProxy(ProcessId.CreateBuilder().SetLabel(0xaa82dfd9).SetEpoch(1497363883))));
            gh.SetGameServer(HostProxyPair.CreateBuilder()
                                .SetHost(ProcessId.CreateBuilder().SetLabel(1277).SetEpoch(1499729371))
                                .SetProxy(ProcessId.CreateBuilder().SetLabel(0xf511871c).SetEpoch(1497363865)));

            var gameFound = GameFactoryManager.FindGame((controller as HandlerController).Client, request, ++GameFactoryManager.RequestIdCounter);
            var clients = (from player in request.Options.PlayerList select GameAccountManager.GetAccountByPersistentID(player.GameAccount.Id) into gameAccount where gameFound != null select gameAccount.LoggedInClient).ToList();

            if (((controller as HandlerController).Client).CurrentChannel != null)
            {
                 var channelStatePermission = bgs.protocol.channel.v1.ChannelState.CreateBuilder()
                    .AddAttribute(bgs.protocol.Attribute.CreateBuilder()
                    .SetName("D3.Party.JoinPermissionPreviousToLock")
                    .SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(1).Build())
                    .Build()).Build();

                var notificationPermission = bgs.protocol.channel.v1.UpdateChannelStateNotification.CreateBuilder()
                    .SetAgentId(((controller as HandlerController).Client).Account.GameAccount.BnetEntityId)
                    .SetStateChange(channelStatePermission)
                    .Build();

                var JoinPerm = bgs.protocol.channel.v1.JoinNotification.CreateBuilder().SetChannelState(channelStatePermission).Build();

                ((controller as HandlerController).Client).MakeTargetedRPC(((controller as HandlerController).Client).CurrentChannel, (lid) => bgs.protocol.channel.v1.ChannelListener.CreateStub(((controller as HandlerController).Client)).OnUpdateChannelState(new HandlerController() { ListenerId = lid }, notificationPermission, callback => { }));
            }            
            gameFound.StartGame(clients, gameFound.DynamicId);
            
            var notificationFound = bgs.protocol.notification.v1.Notification.CreateBuilder()
                .SetSenderId(bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.GameAccountId).SetLow(0).Build())
                .SetTargetId(((controller as HandlerController).Client).Account.GameAccount.BnetEntityId)
                .SetType("GQ_ENTRY");
            var attrF = bgs.protocol.Attribute.CreateBuilder()
                .SetName("game_request_id")
                .SetValue(bgs.protocol.Variant.CreateBuilder().SetUintValue(gameFound.RequestId).Build());
            notificationFound.AddAttribute(attrF);

            ((controller as HandlerController).Client).MakeRPC((lid) =>
                bgs.protocol.notification.v1.NotificationListener.CreateStub(((controller as HandlerController).Client)).OnNotificationReceived(new HandlerController() { ListenerId = lid }, notificationFound.Build(), callback => { }));


            gh.SetGameInstanceId((uint)gameFound.BnetEntityId.Low);

            connectInfo.AddAttribute(bgs.protocol.v2.Attribute.CreateBuilder().SetName("SGameId").SetValue(bgs.protocol.v2.Variant.CreateBuilder().SetIntValue((long)gameFound.BnetEntityId.Low)));
            connectInfo.AddAttribute(bgs.protocol.v2.Attribute.CreateBuilder().SetName("SWorldId").SetValue(bgs.protocol.v2.Variant.CreateBuilder().SetIntValue((long)71150))); // FIXME

            notification.SetRequestId(id);
            notification.SetResult(0);
            notification.SetConnectInfo(connectInfo);
            notification.SetGameHandle(gh);

            System.Threading.Tasks.Task.Delay(2000).ContinueWith(delegate {
                (controller as HandlerController).Client.MakeRPC((lid) => GameRequestListener.CreateStub((controller as HandlerController).Client).OnMatchmakingResult(new HandlerController() { ListenerId = lid }, notification.Build(), callback => { }));
            });
        }
    }
}

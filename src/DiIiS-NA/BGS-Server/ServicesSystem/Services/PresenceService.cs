//Blizzless Project 2022
using System;
using System.Reflection;
using Google.ProtocolBuffers;
using bgs.protocol;
using bgs.protocol.presence.v1;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Helpers;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0xb, serviceName: "bnet.protocol.presence.PresenceService")]
    public class PresenceService : bgs.protocol.presence.v1.PresenceService, IServerService
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        public override void BatchSubscribe(IRpcController controller, BatchSubscribeRequest request, Action<BatchSubscribeResponse> done)
        {
            
            var response = BatchSubscribeResponse.CreateBuilder();
            var EntityId = request.EntityIdList[0];
            response.AddSubscribeFailed(SubscribeResult.CreateBuilder().SetEntityId(request.EntityIdList[0]).SetResult(0));
            Task.Run(() =>
            {

                foreach (var req in request.EntityIdList)
                {
                    switch (req.GetHighIdType())
                    {
                        case EntityIdHelper.HighIdType.AccountId:
                        {
                            var account = AccountManager.GetAccountByPersistentID(req.Low);
                            if (account != null)
                            {
                                Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController)controller).Client), account);
                                account.AddSubscriber((((HandlerController)controller).Client), request.ObjectId);
                                response.AddSubscribeFailed(SubscribeResult.CreateBuilder().SetEntityId(req)
                                    .SetResult(0));
                            }
                        }
                            break;
                        case EntityIdHelper.HighIdType.GameAccountId:
                        {
                            var gameAccount = GameAccountManager.GetAccountByPersistentID(req.Low);
                            if (gameAccount != null)
                            {
                                Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController)controller).Client),
                                    gameAccount);
                                gameAccount.AddSubscriber((((HandlerController)controller).Client), request.ObjectId);
                                response.AddSubscribeFailed(SubscribeResult.CreateBuilder().SetEntityId(req)
                                    .SetResult(0));
                            }
                        }
                            break;
                        default:
                            Logger.Warn("Received an unhandled Presence.Subscribe request with type {0} (0x{1})", req.GetHighIdType(), req.High.ToString("X16"));
                            break;
                    }
                }
                //*/
            });

            done(response.Build());

            //throw new NotImplementedException();
        }

        public override void BatchUnsubscribe(IRpcController controller, BatchUnsubscribeRequest request, Action<NoData> done)
        {
            Logger.Fatal("Batch Unsubscribe not implemented");
        }

        public override void Query(IRpcController controller, QueryRequest request, Action<QueryResponse> done)
        {
            var builder = QueryResponse.CreateBuilder();

            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                {
                    var gameAccount = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    foreach (var key in request.KeyList)
                    {
                        Logger.MethodTrace(MethodBase.GetCurrentMethod(),"{0} {1} - {2}, {3}, {4}", (((HandlerController)controller).Client),
                            gameAccount, (FieldKeyHelper.Program)key.Program, (FieldKeyHelper.OriginatingClass)key.Group,
                            key.Field);
                        var field = gameAccount.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }
                }
                    break;

                case EntityIdHelper.HighIdType.GameAccountId:
                {
                    var gameAccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    foreach (var key in request.KeyList)
                    {
                        Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1} - {2}, {3}, {4}", (((HandlerController)controller).Client),
                            gameAccount, (FieldKeyHelper.Program)key.Program,
                            (FieldKeyHelper.OriginatingClass)key.Group, key.Field);
                        var field = gameAccount.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }
                }
                    break;
                default:
                    Logger.Warn("Received an unhandled Presence.Query request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                    break;
            }

            done(builder.Build());
        }

        public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<NoData> done)
        {
            Task.Run(() =>
            {
                switch (request.EntityId.GetHighIdType())
                {
                    case EntityIdHelper.HighIdType.AccountId:
                    {
                        var gameAccount = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                        if (gameAccount != null)
                        {
                            Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController)controller).Client), gameAccount);
                            gameAccount.AddSubscriber((((HandlerController)controller).Client), request.ObjectId);
                        }
                    }
                        break;
                    case EntityIdHelper.HighIdType.GameAccountId:
                    {
                        var gameaccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                        if (gameaccount != null)
                        {
                            Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController)controller).Client), gameaccount);
                            gameaccount.AddSubscriber((((HandlerController)controller).Client), request.ObjectId);
                        }
                    }
                        break;
                    default:
                        Logger.Warn("Received an unhandled Presence.Subscribe request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                        break;
                }
            });

            var builder = NoData.CreateBuilder();
            done(builder.Build());

        }

        public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
        {
            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                {
                    var gameAccount = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    // The client will probably make sure it doesn't unsubscribe to a null ID, but just to make sure..
                    if (gameAccount != null)
                    {
                        gameAccount.RemoveSubscriber((((HandlerController) controller).Client));
                        Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController) controller).Client), gameAccount);
                    }
                }
                    break;
                case EntityIdHelper.HighIdType.GameAccountId:
                {
                    var gameAccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (gameAccount != null)
                    {
                        gameAccount.RemoveSubscriber((((HandlerController) controller).Client));
                        Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0} {1}", (((HandlerController) controller).Client), gameAccount);
                    }
                }
                    break;
                default:
                    Logger.Warn("Received an unhandled Presence.Unsubscribe request with type {0} (0x{1})",
                        request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                    break;
            }

            var builder = NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Update(IRpcController controller, UpdateRequest request, Action<NoData> done)
        {
            //4,1
            //4,2
            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                {
                    if (request.EntityId.Low <= 0) break;
                    var gameAccount = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (gameAccount == null) break;
                    var traceData = $"{(((HandlerController)controller).Client)} {gameAccount} - {request.FieldOperationCount} Operations";
                    foreach (var fieldOp in request.FieldOperationList)
                    {
                        traceData += $"\t{(FieldKeyHelper.Program)fieldOp.Field.Key.Program}, {(FieldKeyHelper.OriginatingClass)fieldOp.Field.Key.Group}, {fieldOp.Field.Key.Field}";
                    }

                    gameAccount.Update(request.FieldOperationList);
                    Logger.MethodTrace(MethodBase.GetCurrentMethod(), traceData);
                }
                    break;
                case EntityIdHelper.HighIdType.GameAccountId:
                {
                    if (request.EntityId.Low <= 0) break;
                    var gameAccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (gameAccount == null) break;
                    var traceData =
                        $"{(((HandlerController) controller).Client)} {gameAccount} - {request.FieldOperationCount} Operations";
                    foreach (var fieldOp in request.FieldOperationList)
                    {
                        traceData +=
                            $"\t{(FieldKeyHelper.Program)fieldOp.Field.Key.Program}, {(FieldKeyHelper.OriginatingClass)fieldOp.Field.Key.Group}, {fieldOp.Field.Key.Field}";
                    }

                    gameAccount.Update(request.FieldOperationList);
                    Logger.MethodTrace(MethodBase.GetCurrentMethod(), traceData);
                    break;
                }
                default:
                    Logger.Warn("Received an unhandled Presence.Update request with type {0} (0x{1})",
                        request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                    break;
            }

            var builder = NoData.CreateBuilder();
            done(builder.Build());
        }

    }
}

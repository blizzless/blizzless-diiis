//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.presence.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0xb, serviceName: "bnet.protocol.presence.PresenceService")]//: )]
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
                            var account = AccountManager.GetAccountByPersistentID(req.Low);
                            if (account != null)
                            {
                                Logger.Trace("Subscribe() {0} {1}", ((controller as HandlerController).Client), account);
                                account.AddSubscriber(((controller as HandlerController).Client), request.ObjectId);
                                response.AddSubscribeFailed(SubscribeResult.CreateBuilder().SetEntityId(req).SetResult(0));
                            }

                            break;
                        case EntityIdHelper.HighIdType.GameAccountId:
                            var gameaccount = GameAccountManager.GetAccountByPersistentID(req.Low);
                            if (gameaccount != null)
                            {
                                Logger.Trace("Subscribe() {0} {1}", ((controller as HandlerController).Client), gameaccount);
                                gameaccount.AddSubscriber(((controller as HandlerController).Client), request.ObjectId);
                                response.AddSubscribeFailed(SubscribeResult.CreateBuilder().SetEntityId(req).SetResult(0));
                            }
                            break;
                        default:
                            Logger.Warn("Recieved an unhandled Presence.Subscribe request with type {0} (0x{1})", req.GetHighIdType(), req.High.ToString("X16"));
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
            throw new NotImplementedException();
        }

        public override void Query(IRpcController controller, QueryRequest request, Action<QueryResponse> done)
        {
            var builder = bgs.protocol.presence.v1.QueryResponse.CreateBuilder();

            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    foreach (var key in request.KeyList)
                    {
                        Logger.Trace("Query() {0} {1} - {2}, {3}, {4}", ((controller as HandlerController).Client), account, (FieldKeyHelper.Program)key.Program, (FieldKeyHelper.OriginatingClass)key.Group, key.Field);
                        var field = account.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }
                    break;

                case EntityIdHelper.HighIdType.GameAccountId:
                    var gameaccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    foreach (var key in request.KeyList)
                    {
                        Logger.Trace("Query() {0} {1} - {2}, {3}, {4}", ((controller as HandlerController).Client), gameaccount, (FieldKeyHelper.Program)key.Program, (FieldKeyHelper.OriginatingClass)key.Group, key.Field);
                        var field = gameaccount.QueryField(key);
                        if (field != null) builder.AddField(field);
                    }
                    break;
                default:
                    Logger.Warn("Recieved an unhandled Presence.Query request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                    break;
            }

            done(builder.Build());
        }

        public override void Subscribe(IRpcController controller, bgs.protocol.presence.v1.SubscribeRequest request, Action<NoData> done)
        {
            Task.Run(() =>
            {
                switch (request.EntityId.GetHighIdType())
                {
                    case EntityIdHelper.HighIdType.AccountId:
                        var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                        if (account != null)
                        {
                            Logger.Trace("Subscribe() {0} {1}", ((controller as HandlerController).Client), account);
                            account.AddSubscriber(((controller as HandlerController).Client), request.ObjectId);
                        }
                        break;
                    case EntityIdHelper.HighIdType.GameAccountId:
                        var gameaccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                        if (gameaccount != null)
                        {
                            Logger.Trace("Subscribe() {0} {1}", ((controller as HandlerController).Client), gameaccount);
                            gameaccount.AddSubscriber(((controller as HandlerController).Client), request.ObjectId);
                        }
                        break;
                    default:
                        Logger.Warn("Recieved an unhandled Presence.Subscribe request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                        break;
                }
            });

            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());

        }

        public override void Unsubscribe(IRpcController controller, bgs.protocol.presence.v1.UnsubscribeRequest request, Action<NoData> done)
        {
            switch (request.EntityId.GetHighIdType())
            {
                case EntityIdHelper.HighIdType.AccountId:
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    // The client will probably make sure it doesn't unsubscribe to a null ID, but just to make sure..
                    if (account != null)
                    {
                        account.RemoveSubscriber(((controller as HandlerController).Client));
                        Logger.Trace("Unsubscribe() {0} {1}", ((controller as HandlerController).Client), account);
                    }
                    break;
                case EntityIdHelper.HighIdType.GameAccountId:
                    var gameaccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (gameaccount != null)
                    {
                        gameaccount.RemoveSubscriber(((controller as HandlerController).Client));
                        Logger.Trace("Unsubscribe() {0} {1}", ((controller as HandlerController).Client), gameaccount);
                    }
                    break;
                default:
                    Logger.Warn("Recieved an unhandled Presence.Unsubscribe request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                    break;
            }

            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

        public override void Update(IRpcController controller, UpdateRequest request, Action<NoData> done)
        {
            //4,1
            //4,2
            switch(request.EntityId.GetHighIdType())
            {
				case EntityIdHelper.HighIdType.AccountId:
                    if (request.EntityId.Low <= 0) break;
                    var account = AccountManager.GetAccountByPersistentID(request.EntityId.Low);
                    if (account == null) break;
                    var a_trace = string.Format("Update() {0} {1} - {2} Operations", ((controller as HandlerController).Client), account, request.FieldOperationCount);
                    foreach (var fieldOp in request.FieldOperationList)
                    {
                        a_trace += string.Format("\t{0}, {1}, {2}", (FieldKeyHelper.Program)fieldOp.Field.Key.Program, (FieldKeyHelper.OriginatingClass)fieldOp.Field.Key.Group, fieldOp.Field.Key.Field);
                    }
                    account.Update(request.FieldOperationList);
                    Logger.Trace(a_trace);
                    break;
				case EntityIdHelper.HighIdType.GameAccountId:
					if (request.EntityId.Low <= 0) break;
                var gameaccount = GameAccountManager.GetAccountByPersistentID(request.EntityId.Low);
                if (gameaccount == null) break;
                var ga_trace = string.Format("Update() {0} {1} - {2} Operations", ((controller as HandlerController).Client), gameaccount, request.FieldOperationCount);
                foreach (var fieldOp in request.FieldOperationList)
                {
                    ga_trace += string.Format("\t{0}, {1}, {2}", (FieldKeyHelper.Program)fieldOp.Field.Key.Program, (FieldKeyHelper.OriginatingClass)fieldOp.Field.Key.Group, fieldOp.Field.Key.Field);
                }
                gameaccount.Update(request.FieldOperationList);
                Logger.Trace(ga_trace);
                break;
                default:
					Logger.Warn("Recieved an unhandled Presence.Update request with type {0} (0x{1})", request.EntityId.GetHighIdType(), request.EntityId.High.ToString("X16"));
                break;
            }

            var builder = bgs.protocol.NoData.CreateBuilder();
            done(builder.Build());
        }

    }
}

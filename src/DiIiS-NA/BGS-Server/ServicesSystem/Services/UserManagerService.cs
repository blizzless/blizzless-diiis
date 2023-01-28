//Blizzless Project 2022
using bgs.protocol;
using bgs.protocol.user_manager.v1;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.FriendsSystem;
using DiIiS_NA.LoginServer.Helpers;
using Google.ProtocolBuffers;
using System;
using System.Reflection;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x35, serviceName: "bnet.protocol.user_manager.UserManagerService")]
    public class UserManagerService : bgs.protocol.user_manager.v1.UserManagerService, IServerService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<SubscribeResponse> done)
		{
			Logger.MethodTrace(MethodBase.GetCurrentMethod(), "{0}", (((HandlerController) controller).Client));

			UserManager.Instance.AddSubscriber((((HandlerController) controller).Client), request.ObjectId);

			var builder = SubscribeResponse.CreateBuilder();

			var blockedIds = ((HandlerController) controller).Client.Account.IgnoreIds;

			foreach (var blocked in blockedIds)
			{
				var blockedAccount = AccountManager.GetAccountByPersistentID(blocked);
				var blockedPlayer = BlockedPlayer.CreateBuilder()
					.SetAccountId(EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(blockedAccount.PersistentID))
					.SetName(blockedAccount.BattleTag)
					.Build();
				builder.AddBlockedPlayers(blockedPlayer);
			}

			done(builder.Build());
		}

		public override void AddRecentPlayers(IRpcController controller, AddRecentPlayersRequest request, Action<NoData> done)
		{
			Logger.MethodTrace(MethodBase.GetCurrentMethod());
			done(NoData.DefaultInstance);
		}

		public override void ClearRecentPlayers(IRpcController controller, ClearRecentPlayersRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}

		public override void BlockPlayer(IRpcController controller, BlockPlayerRequest request, Action<NoData> done)
		{
			Logger.MethodTrace(MethodBase.GetCurrentMethod());
			done(NoData.CreateBuilder().Build());

			UserManager.BlockAccount((((HandlerController) controller).Client), request);
		}

		public override void UnblockPlayer(IRpcController controller, UnblockPlayerRequest request, Action<NoData> done)
		{
			Logger.MethodTrace(MethodBase.GetCurrentMethod());
			done(NoData.CreateBuilder().Build());

			UserManager.UnblockAccount((((HandlerController) controller).Client), request);
		}

        public override void BlockPlayerForSession(IRpcController controller, BlockPlayerRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }

        public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
        {
            throw new NotImplementedException();
        }
    }
}

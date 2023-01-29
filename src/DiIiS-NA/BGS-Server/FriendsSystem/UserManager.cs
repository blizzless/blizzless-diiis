//Blizzless Project 2022
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.Helpers;
using DiIiS_NA.LoginServer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer.FriendsSystem
{
	public class UserManager : RPCObject
	{
		private static readonly UserManager _instance = new UserManager();
		public static UserManager Instance { get { return _instance; } }

		public static void BlockAccount(BattleClient client, bgs.protocol.user_manager.v1.BlockPlayerRequest request)
		{
			var blocked = GameAccountManager.GetAccountByPersistentID(request.TargetId.Low);
			var blocker = client.Account;

			if (!blocker.IgnoreIds.Contains(blocked.Owner.PersistentID))
				blocker.IgnoreIds.Add(blocked.Owner.PersistentID);
			AddIgnoreToDB(blocker, blocked.Owner);

			var blockedPlayer = bgs.protocol.user_manager.v1.BlockedPlayer.CreateBuilder()
				.SetAccountId(bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(blocked.Owner.PersistentID))
				.SetName(blocked.Owner.BattleTag)
				//.AddRole(0)
				.Build();

			var notifyBlock = bgs.protocol.user_manager.v1.BlockedPlayerAddedNotification.CreateBuilder();
			notifyBlock.SetPlayer(blockedPlayer);
			notifyBlock.SetGameAccountId(blocked.BnetEntityId);

			client.MakeTargetedRpc(UserManager.Instance, (lid) =>
				bgs.protocol.user_manager.v1.UserManagerListener.CreateStub(client).OnBlockedPlayerAdded(new HandlerController() { ListenerId = lid }, notifyBlock.Build(), callback => { }));
		}

		public static void UnblockAccount(BattleClient client, bgs.protocol.user_manager.v1.UnblockPlayerRequest request)
		{
			var blocked = AccountManager.GetAccountByPersistentID(request.TargetId.Low);
			var blocker = client.Account;


			if (blocker.IgnoreIds.Contains(blocked.PersistentID))
				blocker.IgnoreIds.Remove(blocked.PersistentID);
			RemoveIgnoreFromDB(blocker, blocked);

			var blockedPlayer = bgs.protocol.user_manager.v1.BlockedPlayer.CreateBuilder()
				.SetAccountId(bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(blocked.PersistentID))
				.SetName(blocked.BattleTag)
				.Build();

			var notifyUnblock = bgs.protocol.user_manager.v1.BlockedPlayerRemovedNotification.CreateBuilder();
			notifyUnblock.SetPlayer(blockedPlayer);

			client.MakeTargetedRpc(UserManager.Instance, (lid) =>
				bgs.protocol.user_manager.v1.UserManagerListener.CreateStub(client).OnBlockedPlayerRemoved(new HandlerController() { ListenerId = lid }, notifyUnblock.Build(), callback => { }));
		}

		private static void AddIgnoreToDB(Account owner, Account target)
		{
			Logger.MethodTrace($": owner {owner.PersistentID}, target {target.PersistentID}");
			try
			{
				if (DBSessions.SessionQueryWhere<DBAccountLists>(dbl => dbl.ListOwner.Id == owner.PersistentID && dbl.ListTarget.Id == target.PersistentID && dbl.Type == "IGNORE").Any()) return;

				var blockRecord = new DBAccountLists
				{
					ListOwner = owner.DBAccount,
					ListTarget = target.DBAccount,
					Type = "IGNORE"
				};
				DBSessions.SessionSave(blockRecord);
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UserManager.AddIgnoreToDB()");
			}
		}

		private static void RemoveIgnoreFromDB(Account owner, Account target)
		{
			Logger.MethodTrace($": owner {owner.PersistentID}, target {target.PersistentID}");
			try
			{
				var blockRecords = DBSessions.SessionQueryWhere<DBAccountLists>(dbl => dbl.ListOwner.Id == owner.PersistentID && dbl.ListTarget.Id == target.PersistentID && dbl.Type == "IGNORE");
				foreach (var rec in blockRecords)
					DBSessions.SessionDelete(rec);
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "UserManager.RemoveIgnoreFromDB()");
			}
		}

	}
}

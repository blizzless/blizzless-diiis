//Blizzless Project 2022
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage;
//Blizzless Project 2022 
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Base;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Helpers;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Objects;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;

namespace DiIiS_NA.LoginServer.FriendsSystem
{
	public class FriendManager : RPCObject
	{
		private static readonly FriendManager _instance = new FriendManager();
		public static FriendManager Instance { get { return _instance; } }

	
		public static readonly Dictionary<ulong, bgs.protocol.friends.v1.ReceivedInvitation> OnGoingInvitations =
			new Dictionary<ulong, bgs.protocol.friends.v1.ReceivedInvitation>();

		public static ulong InvitationIdCounter = 1;

		static FriendManager()
		{
			_instance.BnetEntityId = bgs.protocol.EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.Unknown).SetLow(0x0000000110000000L + 1).Build();
		}


		public static bool AreFriends(Account account1, Account account2)
		{

			foreach (ulong friendId in account1.FriendsIds)
			{
				if (friendId == account2.PersistentID) return true;
			}
			return false;
		}

		public static bool InvitationExists(Account inviter, Account invitee)
		{
			foreach (var invitation in OnGoingInvitations.Values)
			{
				if ((invitation.InviterIdentity.AccountId == inviter.BnetEntityId) && (invitation.InviteeIdentity.AccountId == invitee.BnetEntityId))
					return true;
			}
			return false;
		}
		//Done
		public static void HandleInvitation(BattleClient client, bgs.protocol.friends.v1.ReceivedInvitation invitation)
		{
			var invitee = AccountManager.GetAccountByPersistentID(invitation.InviteeIdentity.AccountId.Low);
			if (invitee == null) return;

			if (OnGoingInvitations.Values.Any(oldInvite => (oldInvite.InviteeIdentity.AccountId == invitation.InviteeIdentity.AccountId) && (oldInvite.InviterIdentity.AccountId == invitation.InviterIdentity.AccountId)))
				return;

			OnGoingInvitations.Add(invitation.Id, invitation); // track ongoing invitations so we can tranport it forth and back.

			if (invitee.IsOnline)
			{
				var inviter = AccountManager.GetAccountByPersistentID(invitation.InviterIdentity.AccountId.Low);

				var notification = bgs.protocol.friends.v1.InvitationNotification.CreateBuilder()
					.SetInvitation(invitation)
					.SetAccountId(invitee.BnetEntityId);

				invitee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(invitee.GameAccount.LoggedInClient).OnReceivedInvitationAdded(new HandlerController() { ListenerId = lid }, notification.Build(), callback =>
					{
					}));
			}
		}
		//Done
		public static void HandleIgnore(BattleClient client, bgs.protocol.friends.v1.IgnoreInvitationRequest request)
		{
			var invitation = OnGoingInvitations[request.InvitationId];

			var inviter = AccountManager.GetAccountByPersistentID(invitation.InviterIdentity.AccountId.Low);
			var invitee = AccountManager.GetAccountByPersistentID(invitation.InviteeIdentity.AccountId.Low);


			var declinedNotification = bgs.protocol.friends.v1.InvitationNotification.CreateBuilder()
				.SetInvitation(invitation)
				.SetAccountId(invitee.BnetEntityId)
				.SetReason((uint)InvitationRemoveReason.Ignored).Build();

			if (inviter.GameAccount.IsOnline)
			{
				inviter.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(inviter.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, declinedNotification, callback => { }));
			}

			if (invitee.GameAccount.IsOnline)
			{
				invitee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(invitee.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, declinedNotification, callback => { }));
			}

			OnGoingInvitations.Remove(request.InvitationId);
		}
		//Done
		public static void HandleAccept(BattleClient client, bgs.protocol.friends.v1.AcceptInvitationRequest request)
		{
			if (!OnGoingInvitations.ContainsKey(request.InvitationId)) return;
			var invitation = OnGoingInvitations[request.InvitationId];

			var inviter = AccountManager.GetAccountByPersistentID(invitation.InviterIdentity.AccountId.Low);
			var invitee = AccountManager.GetAccountByPersistentID(invitation.InviteeIdentity.AccountId.Low);
			var inviteeAsFriend = bgs.protocol.friends.v1.Friend.CreateBuilder()
				.SetAccountId(invitation.InviteeIdentity.AccountId)
				.AddRole(2)
				.SetPrivileges(3)
				.Build();
			var inviterAsFriend = bgs.protocol.friends.v1.Friend.CreateBuilder()
				.SetAccountId(invitation.InviterIdentity.AccountId)
				.AddRole(2)
				.SetPrivileges(3)
				.Build();

			var notificationToInviter = bgs.protocol.friends.v1.InvitationNotification.CreateBuilder()
				.SetAccountId(inviter.BnetEntityId)
				.SetInvitation(invitation)
				.SetReason((uint)InvitationRemoveReason.Accepted)
				.Build();

			var notificationToInvitee = bgs.protocol.friends.v1.InvitationNotification.CreateBuilder()
				.SetAccountId(invitee.BnetEntityId)
				.SetInvitation(invitation)
				.SetReason((uint)InvitationRemoveReason.Accepted)
				.Build();

			if (!inviter.FriendsIds.Contains(invitee.PersistentID))
				inviter.FriendsIds.Add(invitee.PersistentID);
			AddFriendshipToDB(inviter, invitee);

			// send friend added notifications
			var friendAddedNotificationToInviter = bgs.protocol.friends.v1.FriendNotification.CreateBuilder()
				.SetTarget(inviteeAsFriend).SetAccountId(inviter.BnetEntityId).Build();
			var friendAddedNotificationToInvitee = bgs.protocol.friends.v1.FriendNotification.CreateBuilder()
				.SetTarget(inviterAsFriend).SetAccountId(invitee.BnetEntityId).Build();

			if (inviter.GameAccount.IsOnline)
			{
				inviter.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(inviter.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, notificationToInviter, callback => { }));

				inviter.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(inviter.GameAccount.LoggedInClient).OnFriendAdded(new HandlerController() { ListenerId = lid }, friendAddedNotificationToInviter, callback => { }));
			}

			if (invitee.GameAccount.IsOnline)
			{
				invitee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(invitee.GameAccount.LoggedInClient).OnFriendAdded(new HandlerController() { ListenerId = lid }, friendAddedNotificationToInvitee, callback => { }));

				invitee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(invitee.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, notificationToInvitee, callback => { }));
			}

			OnGoingInvitations.Remove(request.InvitationId);
		}

		public static void HandleDecline(BattleClient client, bgs.protocol.friends.v1.DeclineInvitationRequest request)
		{
			if (!OnGoingInvitations.ContainsKey(request.InvitationId)) return;
			var invitation = OnGoingInvitations[request.InvitationId];

			var inviter = AccountManager.GetAccountByPersistentID(invitation.InviterIdentity.AccountId.Low);
			var invitee = AccountManager.GetAccountByPersistentID(invitation.InviteeIdentity.AccountId.Low);

			var declinedNotification = bgs.protocol.friends.v1.InvitationNotification.CreateBuilder()
				.SetInvitation(invitation)
				.SetReason((uint)InvitationRemoveReason.Declined).Build();

			if (inviter.GameAccount.IsOnline)
			{
				inviter.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(inviter.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, declinedNotification, callback => { }));
			}

			if (invitee.GameAccount.IsOnline)
			{
				invitee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) =>
					bgs.protocol.friends.v1.FriendsListener.CreateStub(invitee.GameAccount.LoggedInClient).OnReceivedInvitationRemoved(new HandlerController() { ListenerId = lid }, declinedNotification, callback => { }));
			}

			OnGoingInvitations.Remove(request.InvitationId);
		}

		public static void HandleRemove(BattleClient client, bgs.protocol.friends.v1.RemoveFriendRequest request)
		{
			var removee = AccountManager.GetAccountByPersistentID(request.TargetId.Low);
			var remover = client.Account;

			var removeeAsFriend = bgs.protocol.friends.v1.Friend.CreateBuilder()
				.SetAccountId(removee.BnetEntityId)
				.SetPrivileges(1)
				.AddRole(1)
				.Build();
			var removerAsFriend = bgs.protocol.friends.v1.Friend.CreateBuilder()
				.SetAccountId(remover.BnetEntityId)
				.SetPrivileges(1)
				.AddRole(1)
				.Build();

			if (remover.FriendsIds.Contains(removee.PersistentID))
				remover.FriendsIds.Remove(removee.PersistentID);
			RemoveFriendshipFromDB(remover, removee);

			var notifyRemover = bgs.protocol.friends.v1.FriendNotification.CreateBuilder()
				.SetTarget(removeeAsFriend)
				.SetAccountId(remover.BnetEntityId)
				.Build();

			client.MakeTargetedRPC(FriendManager.Instance, (lid) =>
				bgs.protocol.friends.v1.FriendsListener.CreateStub(client).OnFriendRemoved(new HandlerController() { ListenerId = lid }, notifyRemover, callback => { }));

			if (removee.GameAccount.IsOnline)
			{
				var notifyRemovee = bgs.protocol.friends.v1.FriendNotification.CreateBuilder().SetTarget(removerAsFriend).SetAccountId(removee.BnetEntityId).Build();
				removee.GameAccount.LoggedInClient.MakeTargetedRPC(FriendManager.Instance, (lid) => bgs.protocol.friends.v1.FriendsListener.CreateStub(removee.GameAccount.LoggedInClient).OnFriendRemoved(new HandlerController() { ListenerId = lid }, notifyRemovee, callback => { }));
			}
		}

		private static void AddFriendshipToDB(Account inviter, Account invitee)
		{
			try
			{
				var inviterRecord = new DBAccountLists
				{
					ListOwner = inviter.DBAccount,
					ListTarget = invitee.DBAccount,
					Type = "FRIEND"
				};
				DBSessions.SessionSave(inviterRecord);

				var inviteeRecord = new DBAccountLists
				{
					ListOwner = invitee.DBAccount,
					ListTarget = inviter.DBAccount,
					Type = "FRIEND"
				};
				DBSessions.SessionSave(inviteeRecord);

			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "FriendManager.AddFriendshipToDB()");
			}
		}

		private static void RemoveFriendshipFromDB(Account remover, Account removee)
		{
			try
			{
				var removerRecords = DBSessions.SessionQueryWhere<DBAccountLists>(dbl => dbl.ListOwner.Id == remover.PersistentID && dbl.ListTarget.Id == removee.PersistentID && dbl.Type == "FRIEND");
				foreach (var rec in removerRecords)
					DBSessions.SessionDelete(rec);

				var removeeRecords = DBSessions.SessionQueryWhere<DBAccountLists>(dbl => dbl.ListOwner.Id == removee.PersistentID && dbl.ListTarget.Id == remover.PersistentID && dbl.Type == "FRIEND");
				foreach (var rec in removeeRecords)
					DBSessions.SessionDelete(rec);
			}
			catch (Exception e)
			{
				Logger.ErrorException(e, "FriendManager.RemoveFriendshipFromDB()");
			}
		}

	}

	public enum InvitationRemoveReason : uint 
	{
		Accepted = 0x0,
		Declined = 0x1,
		Ignored = 0x3
	}
}

using bgs.protocol;
using bgs.protocol.friends.v1;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.FriendsSystem;
using DiIiS_NA.LoginServer.Helpers;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x36, serviceName: "bnet.protocol.friends.FriendsService")]
    public class FriendService : FriendsService, IServerService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<SubscribeResponse> done)
		{
			Logger.Trace("Subscribe() {0}", ((controller as HandlerController).Client));


			FriendManager.Instance.AddSubscriber(((controller as HandlerController).Client), request.ObjectId);

			var builder = SubscribeResponse.CreateBuilder()
				.SetMaxFriends(127)
				.SetMaxReceivedInvitations(127)
				.SetMaxSentInvitations(127)
				.AddRole(Role.CreateBuilder().SetId(1).SetName("battle_tag_friend").Build())
				.AddRole(Role.CreateBuilder().SetId(2).SetName("real_id_friend").Build());

			var friendsIDs = ((controller as HandlerController).Client).Account.FriendsIds;
			foreach (var dbidFriend in friendsIDs) // send friends list.
			{
				var resp = Friend.CreateBuilder()
					.SetAccountId(EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(dbidFriend))
					.SetPrivileges(384)

					.AddRole(1);
				builder.AddFriends(resp.Build());
			}

			var invitations = new List<ReceivedInvitation>();

			foreach (var invitation in FriendManager.OnGoingInvitations.Values)
			{
				if (invitation.InviteeIdentity.AccountId == ((controller as HandlerController).Client).Account.BnetEntityId && !friendsIDs.Contains(invitation.InviterIdentity.AccountId.Low))
				{
					invitations.Add(invitation);
				}
			}

			if (invitations.Count > 0)
				builder.AddRangeReceivedInvitations(invitations);

			done(builder.Build());
		}
		public override void SendInvitation(IRpcController controller, SendInvitationRequest request, Action<NoData> done)
		{
			var extensionBytes = request.Params.UnknownFields.FieldDictionary[103].LengthDelimitedList[0].ToByteArray();
			var friendRequest = FriendInvitationParams.ParseFrom(extensionBytes);

			var response = NoData.CreateBuilder();

			if (friendRequest.TargetEmail.ToLower() == ((controller as HandlerController).Client).Account.Email.ToLower())
			{
				((controller as HandlerController).Status) = 317202;
				done(response.Build());
				return;
			}

			if (friendRequest.TargetBattleTag == ((controller as HandlerController).Client).Account.BattleTag)
			{
				((controller as HandlerController).Status) = 317202;
				done(response.Build());
				return;
			}

			Account invitee;

			Logger.Trace("Friend request body: {0}", friendRequest.ToString());

			if (friendRequest.HasTargetEmail)
				invitee = AccountManager.GetAccountByEmail(friendRequest.TargetEmail);
			else
				invitee = AccountManager.GetAccountByBattletag(friendRequest.TargetBattleTag);

			if (invitee == null)
			{
				if (friendRequest.HasTargetEmail)
					((controller as HandlerController).Status) = 4;
				else
					((controller as HandlerController).Status) = 317203;
				done(response.Build());
				return;
			}
			else if (FriendManager.AreFriends(((controller as HandlerController).Client).Account, invitee))
			{
				if (friendRequest.HasTargetEmail)
					((controller as HandlerController).Status) = 317201;
				else
					((controller as HandlerController).Status) = 5003;
				done(response.Build());
				return;
			}
			else if (FriendManager.InvitationExists(((controller as HandlerController).Client).Account, invitee))
			{
				if (friendRequest.HasTargetEmail)
					((controller as HandlerController).Status) = 317200;
				else
					((controller as HandlerController).Status) = 5005;
				done(response.Build());
				return;
			}
			else if (invitee.IgnoreIds.Contains((controller as HandlerController).Client.Account.PersistentID))
			{
				((controller as HandlerController).Status) = 5006;
				done(response.Build());
				return;
			}

			Logger.Trace("{0} sent {1} friend invitation.", ((controller as HandlerController).Client).Account, invitee);

			var invitation = ReceivedInvitation.CreateBuilder()
				.SetId(FriendManager.InvitationIdCounter++) // we may actually need to store invitation ids in database with the actual invitation there. /raist.				
				.SetInviterIdentity(Identity.CreateBuilder().SetAccountId(((controller as HandlerController).Client).Account.BnetEntityId))
				.SetInviteeIdentity(Identity.CreateBuilder().SetAccountId(invitee.BnetEntityId))
				.SetInviterName(((controller as HandlerController).Client).Account.BattleTagName)
				.SetInviteeName(invitee.BattleTagName)
				.SetCreationTime(DateTime.Now.ToUnixTime())
				.SetUnknownFields(UnknownFieldSet.CreateBuilder()
								.AddField(9, UnknownField.CreateBuilder().AddFixed32(17459).Build())
								.AddField(103, UnknownField.CreateBuilder().AddLengthDelimited(request.Params.UnknownFields.FieldDictionary[103].LengthDelimitedList[0]).Build())
								.Build());

			done(response.Build());

			// notify the invitee on invitation.
			FriendManager.HandleInvitation(((controller as HandlerController).Client), invitation.Build());
			FriendManager.Instance.NotifyUpdate();
			(controller as HandlerController).Client.Account.NotifyUpdate();
			(controller as HandlerController).Client.Account.GameAccount.NotifyUpdate();
		}
		public override void AcceptInvitation(IRpcController controller, AcceptInvitationRequest request, Action<NoData> done)
		{
			Logger.Trace("{0} accepted friend invitation.", ((controller as HandlerController).Client).Account);

			var response = NoData.CreateBuilder();
			done(response.Build());

			FriendManager.HandleAccept(((controller as HandlerController).Client), request);
		}
		public override void RevokeInvitation(IRpcController controller, RevokeInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void DeclineInvitation(IRpcController controller, DeclineInvitationRequest request, Action<NoData> done)
		{
			Logger.Trace("{0} declined friend invitation.", ((controller as HandlerController).Client).Account);

			var response = NoData.CreateBuilder();
			done(response.Build());

			FriendManager.HandleDecline(((controller as HandlerController).Client), request);
		}
		public override void IgnoreInvitation(IRpcController controller, IgnoreInvitationRequest request, Action<NoData> done)
		{

			//throw new NotImplementedException();
			var response = NoData.CreateBuilder();
			done(response.Build());

			FriendManager.HandleIgnore(((controller as HandlerController).Client), request);

		}
		public override void RemoveFriend(IRpcController controller, RemoveFriendRequest request, Action<NoData> done)
		{
			Logger.Trace("{0} removed friend with id {1}.", ((controller as HandlerController).Client).Account, request.TargetId);


			done(NoData.DefaultInstance);

			FriendManager.HandleRemove(((controller as HandlerController).Client), request);
			FriendManager.Instance.NotifyUpdate();
			(controller as HandlerController).Client.Account.NotifyUpdate();
			(controller as HandlerController).Client.Account.GameAccount.NotifyUpdate();

		}
		public override void ViewFriends(IRpcController controller, ViewFriendsRequest request, Action<ViewFriendsResponse> done)
		{
			Logger.Trace("ViewFriends(): {0}.", request.ToString());

			var builder = ViewFriendsResponse.CreateBuilder();
			var friendsIDs = AccountManager.GetAccountByPersistentID(request.TargetId.Low).FriendsIds;
			foreach (var dbidFriend in friendsIDs) // send friends list.
			{
				var friend = AccountManager.GetAccountByPersistentID(dbidFriend);
				var resp = FriendOfFriend.CreateBuilder()
					.SetAccountId(EntityId.CreateBuilder().SetHigh((ulong)EntityIdHelper.HighIdType.AccountId).SetLow(dbidFriend))
					.SetBattleTag(friend.BattleTag)
					.SetFullName(friend.BattleTagName)
					.SetPrivileges(384)
					.AddRole(1);
				builder.AddFriends(resp.Build());
			}
			done(builder.Build());
		}
		public override void UpdateFriendState(IRpcController controller, UpdateFriendStateRequest request, Action<NoData> done)
		{
			Logger.Trace("UpdateFriendState(): {0}.", request.ToString());

			done(NoData.CreateBuilder().Build());
		}
		public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void RevokeAllInvitations(IRpcController controller, RevokeAllInvitationsRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void GetFriendList(IRpcController controller, GetFriendListRequest request, Action<GetFriendListResponse> done)
		{
			throw new NotImplementedException();
		}
		public override void CreateFriendship(IRpcController controller, CreateFriendshipRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void SetAttribute(IRpcController controller, SetAttributeRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}

	}

}

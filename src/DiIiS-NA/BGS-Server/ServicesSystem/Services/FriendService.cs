//Blizzless Project 2022
//Blizzless Project 2022 
using bgs.protocol;
//Blizzless Project 2022 
using bgs.protocol.friends.v1;
//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using Google.ProtocolBuffers;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
    [Service(serviceID: 0x36, serviceName: "bnet.protocol.friends.FriendsService")]
    public class FriendService : bgs.protocol.friends.v1.FriendsService, IServerService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();



		public override void AcceptInvitation(IRpcController controller, AcceptInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void CreateFriendship(IRpcController controller, CreateFriendshipRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void DeclineInvitation(IRpcController controller, DeclineInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void GetFriendList(IRpcController controller, GetFriendListRequest request, Action<GetFriendListResponse> done)
		{
			throw new NotImplementedException();
		}
		public override void IgnoreInvitation(IRpcController controller, IgnoreInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void RemoveFriend(IRpcController controller, RemoveFriendRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void RevokeAllInvitations(IRpcController controller, RevokeAllInvitationsRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void RevokeInvitation(IRpcController controller, RevokeInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void SendInvitation(IRpcController controller, SendInvitationRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void SetAttribute(IRpcController controller, SetAttributeRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void Subscribe(IRpcController controller, SubscribeRequest request, Action<SubscribeResponse> done)
		{
			throw new NotImplementedException();
		}
		public override void Unsubscribe(IRpcController controller, UnsubscribeRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void UpdateFriendState(IRpcController controller, UpdateFriendStateRequest request, Action<NoData> done)
		{
			throw new NotImplementedException();
		}
		public override void ViewFriends(IRpcController controller, ViewFriendsRequest request, Action<ViewFriendsResponse> done)
		{
			throw new NotImplementedException();
		}

	}

}

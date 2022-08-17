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

		
		public static void HandleIgnore(BattleClient client, bgs.protocol.friends.v1.IgnoreInvitationRequest request)
		{
			
		}

		public static void HandleAccept(BattleClient client, bgs.protocol.friends.v1.AcceptInvitationRequest request)
		{
			
		}

		public static void HandleDecline(BattleClient client, bgs.protocol.friends.v1.DeclineInvitationRequest request)
		{
			
		}

		public static void HandleRemove(BattleClient client, bgs.protocol.friends.v1.RemoveFriendRequest request)
		{
			
		}


	}

	public enum InvitationRemoveReason : uint 
	{
		Accepted = 0x0,
		Declined = 0x1,
		Ignored = 0x3
	}
}

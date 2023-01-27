//Blizzless Project 2022
using System;
using System.Collections.Generic;
using bgs.protocol.channel.v1;
using DiIiS_NA.LoginServer.AccountsSystem;

namespace DiIiS_NA.LoginServer.ChannelSystem
{
	public class Member
	{
		// TODO: Need moar!
		public enum Role : uint
		{
			ChannelMember = 1,
			ChannelCreator = 2,
			PartyMember = 100,
			PartyLeader = 101 // There's a cap where no member has Role.ChannelCreator (which is plausible since games are actually channels)
		}

		// TODO: These are flags..
		[Flags]
		public enum Privilege : ulong
		{
			None = 0,
			JoinedMember = 56261, // 0x000000000000DBC5
			UnkJoinedMember = 56261, // 0x000000000000DBC5
			Creator = 64439, // 0x000000000000FBB7
			UnkCreator = 64511, // 0x000000000000FBFF
			Chat = 131072, // 0x0000000000020000
			UnkMember2 = 199552, // 0x0000000000030B80
			Member = 199594, // 0x0000000000030BAA
			UnkMember = 199594, // 0x0000000000030BAA
			Ultra = 0xffffffff
		}

		public bgs.protocol.Identity Identity { get; set; }
		public Channel Channel { get; set; }
		public GameAccount GameAccount { get; set; }
		public Privilege Privileges { get; set; }
		public List<Role> Roles { get; private set; }
		public MemberAccountInfo Info { get; private set; }

		public MemberState BnetMemberState
		{
			get
			{
				var builder = MemberState.CreateBuilder();
				builder.SetInfo(this.Info);
				foreach (var role in this.Roles)
				{
					builder.AddRole((uint)role);
				}

				if (this.Channel.IsGuildChannel)
				{
					var rank = this.Channel.Guild.GetRank(this.Identity.GameAccountId.Low);
					var note = this.Channel.Guild.GetMemberNote(this.Identity.GameAccountId.Low);
					builder.AddAttribute(bgs.protocol.Attribute.CreateBuilder().SetName("D3.GuildMember.Rank").SetValue(bgs.protocol.Variant.CreateBuilder().SetIntValue(rank)));
					builder.AddAttribute(bgs.protocol.Attribute.CreateBuilder().SetName("D3.GuildMember.Note").SetValue(bgs.protocol.Variant.CreateBuilder().SetStringValue(note)));
					builder.AddAttribute(bgs.protocol.Attribute.CreateBuilder().SetName("D3.GuildMember.AchievementPoints").SetValue(bgs.protocol.Variant.CreateBuilder().SetUintValue(this.GameAccount.AchievementPoints)));
				}
				else
				{
										if (this.Privileges != Privilege.None)
											builder.SetPrivileges((ulong)this.Privileges); // We don't have to set this if it is the default (0)
				}
				return builder.Build();
			}
		}

		public bgs.protocol.channel.v1.Member BnetMember
		{
			get
			{
				return bgs.protocol.channel.v1.Member.CreateBuilder()
					.SetIdentity(this.Identity)
					.SetState(this.BnetMemberState)
					.Build();
			}
		}

		public Member(Channel channel, GameAccount account, Privilege privs, params Role[] roles)
		{
			this.Channel = channel;
			this.GameAccount = account;
			this.Identity = bgs.protocol.Identity.CreateBuilder().SetGameAccountId(account.BnetEntityId).Build();
			this.Privileges = privs;
			this.Roles = new List<Role>();
			AddRoles(roles);
			this.Info = MemberAccountInfo.CreateBuilder()
				.SetBattleTag(account.Owner.BattleTag)
				.Build();
		}

		public void AddRoles(params Role[] roles)
		{
			foreach (var role in roles)
				AddRole(role);
		}

		public void AddRole(Role role)
		{
			if (!this.Roles.Contains(role))
				this.Roles.Add(role);
		}
	}
}

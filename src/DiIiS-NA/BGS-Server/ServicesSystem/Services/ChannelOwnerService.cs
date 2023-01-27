//Blizzless Project 2022
using bgs.protocol.channel.v1;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Base;
using DiIiS_NA.LoginServer.ChannelSystem;
using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.LoginServer.ServicesSystem.Services
{
	[Service(serviceID: 0x31, serviceName: "bnet.protocol.channel.ChannelOwner")]
	public class ChannelOwnerService : bgs.protocol.channel.v1.ChannelOwnerService, IServerService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		public override void CreateChannel(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.CreateChannelRequest request, System.Action<bgs.protocol.channel.v1.CreateChannelResponse> done)
		{
			var channel = ChannelManager.CreateNewChannel(((controller as HandlerController).Client), request.ObjectId);
			var builder = bgs.protocol.channel.v1.CreateChannelResponse.CreateBuilder()
				.SetObjectId(channel.DynamicId)
				.SetChannelId(channel.BnetEntityId)
				;

			done(builder.Build());
			channel.SetOwner(((controller as HandlerController).Client));
			channel.AddMember(((controller as HandlerController).Client));
		}
		
		public override void ListChannels(IRpcController controller, ListChannelsRequest request, Action<ListChannelsResponse> done)
		{
			List<Channel> chatChannels = ChannelManager.GetChatChannels();
			var builder = ListChannelsResponse.CreateBuilder();
			
			foreach (Channel channel in chatChannels)
			{
				if (!channel.HasUser(((controller as HandlerController).Client)) && (request.Options.HasName ? request.Options.Name == channel.Name : true) && channel.MaxMembers > channel.Members.Count)
					builder.AddChannel(bgs.protocol.channel.v1.ChannelDescription.CreateBuilder().SetCurrentMembers((uint)channel.Members.Count)
						.SetChannelId(bgs.protocol.EntityId.CreateBuilder().SetHigh(channel.BnetEntityId.High).SetLow(channel.BnetEntityId.Low))
						.SetState(channel.State));
			}

			done(builder.Build());
		}
		public override void GetChannelInfo(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.GetChannelInfoRequest request, System.Action<bgs.protocol.channel.v1.GetChannelInfoResponse> done)
		{
			var builder = bgs.protocol.channel.v1.GetChannelInfoResponse.CreateBuilder();
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);

			if (channel != null)
				builder.SetChannelInfo(channel.Info);
			else
				Logger.Warn("Channel does not exist!");

			done(builder.Build());
		}

		public override void JoinChannel(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.JoinChannelRequest request, System.Action<bgs.protocol.channel.v1.JoinChannelResponse> done)
		{
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);

			channel.Join(((controller as HandlerController).Client), request.ObjectId);
			var builder = bgs.protocol.channel.v1.JoinChannelResponse.CreateBuilder().SetObjectId(channel.DynamicId).SetMemberId((controller as HandlerController).Client.Account.BnetEntityId);
			
			((controller as HandlerController).Client).ChatChannels.Add(channel);
			
			done(builder.Build());
		}

        

        public override void SubscribeChannel(Google.ProtocolBuffers.IRpcController controller, bgs.protocol.channel.v1.SubscribeChannelRequest request, Action<bgs.protocol.channel.v1.SubscribeChannelResponse> done)
		{
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);
			var builder = bgs.protocol.channel.v1.SubscribeChannelResponse.CreateBuilder();
			
			builder.SetObjectId(channel.DynamicId);
			done(builder.Build());
			
			((controller as HandlerController).Client).ChatChannels.Add(channel);
			channel.Join(((controller as HandlerController).Client), request.ObjectId);

		}
	}
}

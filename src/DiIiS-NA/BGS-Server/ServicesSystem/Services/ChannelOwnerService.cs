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

		public override void CreateChannel(IRpcController controller, CreateChannelRequest request, Action<CreateChannelResponse> done)
		{
			var channel = ChannelManager.CreateNewChannel((((HandlerController) controller).Client), request.ObjectId);
			var builder = CreateChannelResponse.CreateBuilder()
				.SetObjectId(channel.DynamicId)
				.SetChannelId(channel.BnetEntityId)
				;

			done(builder.Build());
			channel.SetOwner((((HandlerController) controller).Client));
			channel.AddMember((((HandlerController) controller).Client));
		}
		
		public override void ListChannels(IRpcController controller, ListChannelsRequest request, Action<ListChannelsResponse> done)
		{
			List<Channel> chatChannels = ChannelManager.GetChatChannels();
			var builder = ListChannelsResponse.CreateBuilder();
			
			foreach (Channel channel in chatChannels)
			{
				if (!channel.HasUser((((HandlerController) controller).Client)) && (request.Options.HasName ? request.Options.Name == channel.Name : true) && channel.MaxMembers > channel.Members.Count)
					builder.AddChannel(ChannelDescription.CreateBuilder().SetCurrentMembers((uint)channel.Members.Count)
						.SetChannelId(bgs.protocol.EntityId.CreateBuilder().SetHigh(channel.BnetEntityId.High).SetLow(channel.BnetEntityId.Low))
						.SetState(channel.State));
			}

			done(builder.Build());
		}
		public override void GetChannelInfo(IRpcController controller, GetChannelInfoRequest request, Action<GetChannelInfoResponse> done)
		{
			var builder = GetChannelInfoResponse.CreateBuilder();
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);

			if (channel != null)
				builder.SetChannelInfo(channel.Info);
			else
				Logger.Warn("Channel does not exist!");

			done(builder.Build());
		}

		public override void JoinChannel(IRpcController controller, JoinChannelRequest request, Action<JoinChannelResponse> done)
		{
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);

			channel.Join((((HandlerController) controller).Client), request.ObjectId);
			var builder = JoinChannelResponse.CreateBuilder().SetObjectId(channel.DynamicId).SetMemberId(((HandlerController) controller).Client.Account.BnetEntityId);
			
			(((HandlerController) controller).Client).ChatChannels.Add(channel);
			
			done(builder.Build());
		}

        

        public override void SubscribeChannel(IRpcController controller, SubscribeChannelRequest request, Action<SubscribeChannelResponse> done)
		{
			var channel = ChannelManager.GetChannelByEntityId(request.ChannelId);
			var builder = SubscribeChannelResponse.CreateBuilder();
			
			builder.SetObjectId(channel.DynamicId);
			done(builder.Build());
			
			(((HandlerController) controller).Client).ChatChannels.Add(channel);
			channel.Join((((HandlerController) controller).Client), request.ObjectId);

		}
	}
}

//Blizzless Project 2022
using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.GamesSystem;
using DiIiS_NA.LoginServer.GuildSystem;
using DiIiS_NA.LoginServer.Helpers;
using System;
using System.Collections.Generic;

namespace DiIiS_NA.LoginServer.ChannelSystem
{
	public static class ChannelManager
	{
		private static readonly Logger Logger = LogManager.CreateLogger();

		private static Dictionary<string, List<Channel>> ChatChannels = new Dictionary<string, List<Channel>>();

		public readonly static Dictionary<ulong, Channel> Channels =
			new Dictionary<ulong, Channel>();

		private static string[] chatNames = new string[]{
			"D3_LookingForGroup",
			"D3_PVP",
			"D3_Hardcore",
			"D3_Trade",
			"D3_Barbarian",
			"D3_Crusader",
			"D3_DemonHunter",
			"D3_Monk",
			"D3_GeneralChat",
			"D3_Wizard",
			"D3_WitchDoctor",
			"D3_Necromancer",
		};

		public static Channel CreateNewChannel(BattleClient client, ulong remoteObjectId)
		{
			var channel = new Channel(client, false, remoteObjectId);
			Channels.Add(channel.DynamicId, channel);
			return channel;
		}

		private static void AddPublicChatChannel(string channelName, uint index)
		{
			var channel = new Channel(null, false, 0, true);
			channel.BucketIndex = index;
			channel.Name = channelName;
			channel.SetOpen();

			if (!ChatChannels.ContainsKey(channelName))
				ChatChannels.Add(channelName, new List<Channel>());
			ChatChannels[channelName].Add(channel);

			Channels.Add(channel.DynamicId, channel);
		}

		public static Channel CreateGuildChannel(Guild guild)
		{
			var channel = new Channel(null, false, 0, true);
			channel.IsGuildChannel = true;
			channel.Guild = guild;
			Channels.Add(channel.DynamicId, channel);
			return channel;
		}

		public static Channel CreateGuildGroupChannel(Guild guild)
		{
			var channel = new Channel(null, false, 0, true);
			channel.BucketIndex = 0;
			channel.Name = guild.Name;
			channel.SetOpen();
			channel.IsGuildChannel = true;
			channel.IsGuildChatChannel = true;
			channel.Guild = guild;
			Channels.Add(channel.DynamicId, channel);
			return channel;
		}

		static ChannelManager()
		{
			uint index = 1;
			foreach (string chatName in chatNames)
			{
				for (int i = 0; i < 10; i++)
				{
					AddPublicChatChannel(chatName, index);
				}
				index++;
			}
		}

		public static List<Channel> GetChatChannels()
		{
			List<Channel> chs = new List<Channel>();
			foreach (var pair in ChatChannels)
				foreach (var channel in pair.Value)
				{
					if (channel.Members.Count < 99)
					{
						chs.Add(channel);
						break;
					}
				}
			return chs;
		}

		public static void AddGameChannel(Channel channel)
		{
			Channels.Add(channel.DynamicId, channel);
		}

		public static void DissolveChannel(ulong id)
		{
			Logger.Debug("Dissolving channel {0}", id);
			if (!Channels.ContainsKey(id))
			{
				Logger.Warn("Attempted to delete a non-existent channel with ID {0}", id);
				return;
			}
			var channel = Channels[id];
			if (channel.IsGameChannel)
				GameFactoryManager.DeleteGame(id);
			channel.RemoveAllMembers();
			//Channels.Remove(id);
		}

		public static Channel GetChannelByEntityId(bgs.protocol.EntityId entityId)
		{
			if (entityId.GetHighIdType() == EntityIdHelper.HighIdType.ChannelId)
			{
				if (Channels.ContainsKey(entityId.Low - 0x0000000100000000L))
					return Channels[entityId.Low - 0x0000000100000000L];
				else if(Channels.ContainsKey(entityId.Low))
					return Channels[entityId.Low];
				else if (Channels.ContainsKey((uint)entityId.Low))
					return Channels[(uint)entityId.Low];
				
			}
			else
				Logger.Warn("Given entity ID doesn't look like a channel ID!");
			return null;
		}

		public static Channel GetChannelByChannelId(bgs.protocol.channel.v1.ChannelId entityId)
		{
			if (Channels.ContainsKey(entityId.Id))
				return Channels[entityId.Id];
			return null;
		}

		public static Channel GetChannelByEntityId(D3.OnlineService.EntityId entityId)
		{
			if (entityId.IdHigh == (ulong)EntityIdHelper.HighIdType.ChannelId)
			{
				if (Channels.ContainsKey(entityId.IdLow))
					return Channels[entityId.IdLow];
				else if (Channels.ContainsKey((uint)entityId.IdLow))
					return Channels[(uint)entityId.IdLow];
			}
			else
				Logger.Warn("Given entity ID doesn't look like a channel ID!");
			return null;
		}

		public static Channel GetChannelByDynamicId(ulong dynamicId)
		{
			if (Channels.ContainsKey(dynamicId))
				return Channels[dynamicId];
			else if (Channels.ContainsKey((uint)dynamicId))
				return Channels[(uint)dynamicId];
			else
				return Channels[dynamicId];
		}
	}
}

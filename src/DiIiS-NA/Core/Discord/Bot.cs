using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiIiS_NA.Core.Discord.Services;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;

namespace DiIiS_NA.Core.Discord
{
    public class Bot
    {
		private static readonly Logger Logger = LogManager.CreateLogger();
		
        public static void Init()
            => new Bot().MainAsync().GetAwaiter().GetResult();
			
		public DiscordSocketClient Client = null;
		private DiscordSocketConfig _config = new DiscordSocketConfig{MessageCacheSize = 100};

        public async Task MainAsync()
        {
            var services = ConfigureServices();
            this.Client = services.GetService<DiscordSocketClient>();
            await services.GetService<CommandHandlingService>().InitializeAsync();

            await this.Client.LoginAsync(TokenType.Bot, Config.Instance.Token);
            await this.Client.StartAsync();

			this.Client.ReactionAdded += HandleReactionAsync;// ReactionAdded;

            //await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
				.AddSingleton(new DiscordSocketClient(_config))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Add additional services here...
                .BuildServiceProvider();
        }
		private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
		{
			if (this.Client.GetUser(reaction.UserId).IsBot) return;

			// If the message was not in the cache, downloading it will result in getting a copy of it.
			var msg = await message.GetOrDownloadAsync();
			if (channel.Id == (ulong)DiIiS_NA.Core.Discord.Config.Instance.EventsChannelId)
			{
				var user = reaction.User.Value;
				var guild_user = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetUser(user.Id);

				if (!guild_user.Roles.Select(r => r.Id).Contains((ulong)DiIiS_NA.Core.Discord.Config.Instance.BaseRoleId) && !guild_user.IsBot)
				{
					await msg.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
					await user.SendMessageAsync("**Your #💎events entry has been removed because your Blizzless account isn't linked to Discord!**\nYou can do that if you send me:\n\n`!email my_d3r_email@something.com`\n\n**(Replace** `my_d3r_email@something.com` **with your D3 Reflection email)**");
				}
			}

		}
		
		public async Task AddCollectorRole(ulong userId)
		{
			try
			{
				var user = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetUser(userId);
				await user.AddRoleAsync(this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetRole((ulong)DiIiS_NA.Core.Discord.Config.Instance.CollectorRoleId));
				await user.SendMessageAsync("Congratulations! You are now a **Collector**.");
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "AddCollectorRole() exception: ");
			}
		}
		
		public async Task AddPremiumRole(ulong userId)
		{
			try
			{
				var user = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetUser(userId);
				await user.AddRoleAsync(this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetRole((ulong)DiIiS_NA.Core.Discord.Config.Instance.PremiumRoleId));
				await user.SendMessageAsync("Congratulations! You are now a **Premium** user.");
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "AddPremiumRole() exception: ");
			}
		}
		
		public async Task UpdateBattleTag(ulong userId, string btag)
		{
			try
			{
				var user = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetUser(userId);
				await user.AddRoleAsync(this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetRole((ulong)DiIiS_NA.Core.Discord.Config.Instance.PremiumRoleId));
				await user.ModifyAsync(x => {x.Nickname = btag;});
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "UpdateBattleTag() exception: ");
			}
		}
		
		public async Task ClearPremiumRoles(List<ulong> userIds)
		{
			try
			{
				var guild = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId);
				var role = guild.GetRole((ulong)DiIiS_NA.Core.Discord.Config.Instance.PremiumRoleId);
				foreach (var userId in userIds)
				{
					try
					{
						var user = guild.GetUser(userId);
						await user.RemoveRoleAsync(role);
					} catch {}
				}
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "ClearPremiumRoles() exception: ");
			}
		}
		
		public async Task ShowServerStats()
		{
			try
			{
				var guild = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId);
				var channelId = (ulong)DiIiS_NA.Core.Discord.Config.Instance.StatsChannelId;
				string opened = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "ServerOpened").First().Value > 0 ? "ENABLED" : "DISABLED";
			//	int gs_count = Program.MooNetServer.MooNetBackend.GameServers.Count;
			//	int ccu = DiIiS_NA.Core.MooNet.Online.PlayerManager.OnlinePlayers.Count;
			//	int games = DiIiS_NA.Core.MooNet.Games.GameFactoryManager.GamesOnline;
				var messages = await guild.GetTextChannel(channelId).GetMessagesAsync(10).FlattenAsync();
				await guild.GetTextChannel(channelId).DeleteMessagesAsync(messages);
			//	await guild.GetTextChannel(channelId).SendMessageAsync($"Login availability: **{opened}**\nGame servers available: {gs_count}\nPlayers online: {ccu}\nGames online: {games}");
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "ShowStats() exception: ");
			}
		}
		
		
		public async Task StartGiveaway()
		{
			try
			{
				var guild = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId);
				var channelId = (ulong)DiIiS_NA.Core.Discord.Config.Instance.EventsChannelId;
				var param_message = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "DiscordGiveawayPostId");
				if (param_message.Count < 1)
				{
					var messages = await guild.GetTextChannel(channelId).GetMessagesAsync(10).FlattenAsync();
					await guild.GetTextChannel(channelId).DeleteMessagesAsync(messages);
				}
				else
				{
					await guild.GetTextChannel(channelId).DeleteMessageAsync(param_message.First().Value);
				}
				
				var eb = new EmbedBuilder();
				eb.WithTitle("Reward: 7 days of Premium");
				eb.WithDescription("Click <:wolfRNG:607868292979490816> to join.\nEnds at 18:00 UTC");
				eb.WithFooter("You must bind your D3R account email to be able to join!");
				eb.WithColor(Color.Blue);
				var mes = await guild.GetTextChannel(channelId).SendMessageAsync("@here \n <:wolfRNG:607868292979490816> **GIVEAWAY ROULETTE!** <:wolfRNG:607868292979490816>", false, eb.Build());
				
				//var giveaway_message = await guild.GetTextChannel(channelId).GetMessagesAsync(1).FlattenAsync();
				//foreach (var mes in giveaway_message)
				//{
					var param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "DiscordGiveawayPostId");
					if (param.Count < 1)
					{
						var new_param = new DBGlobalParams{
							Name = "DiscordGiveawayPostId",
							Value = mes.Id
						};
						DBSessions.SessionSave(new_param);
					}
					else
					{
						var postId = param.First();
						postId.Value = mes.Id;
						DBSessions.SessionUpdate(postId);
					}
					await (mes as IUserMessage).AddReactionAsync(Emote.Parse("<:wolfRNG:607868292979490816>"));
				//}
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "StartGiveaway() exception: ");
			}
		}
		
		public async Task FinishGiveaway()
		{
			try
			{
				var guild = this.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId);
				var channelId = (ulong)DiIiS_NA.Core.Discord.Config.Instance.EventsChannelId;
				bool haveWinner = true;
				string winnerName = "";
				var param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "DiscordGiveawayPostId");
				if (param.Count < 1)
				{
					haveWinner = false;
				}
				else
				{
					if (param.First().Value > 0)
					{
						var message = await guild.GetTextChannel(channelId).GetMessageAsync(param.First().Value);
						var reactedUsers = await (message as IUserMessage).GetReactionUsersAsync(Emote.Parse("<:wolfRNG:607868292979490816>"), 100).FlattenAsync();
						var contestants = reactedUsers.Where(u => !u.IsBot).ToList();
						if (contestants.Count() > 0)
						{
							var winner = reactedUsers.Where(u => !u.IsBot).ToList()[DiIiS_NA.Core.Helpers.Math.FastRandom.Instance.Next(0, reactedUsers.Count() - 1)];
							winnerName = guild.GetUser(winner.Id).Nickname;
							await winner.SendMessageAsync("Congratulations! You have won **7 days of D3 Reflection Premium**!.\nYour account has already had its Premium prolonged. Have a nice game!");
							var acc = AccountManager.GetAccountByDiscordId(winner.Id);
							if (acc != null) {
								//acc.UpdatePremiumTime(7);
							}
						}
						else
							haveWinner = false;
					}
					else
						haveWinner = false;
				}
				
				if (param.Count < 1)
				{
					var messages = await guild.GetTextChannel(channelId).GetMessagesAsync(10).FlattenAsync();
					await guild.GetTextChannel(channelId).DeleteMessagesAsync(messages);
				}
				else
				{
					await guild.GetTextChannel(channelId).DeleteMessageAsync(param.First().Value);
				}
				
				var eb = new EmbedBuilder();
				eb.WithTitle("Giveaway ended");
				eb.WithDescription(haveWinner ? string.Format("Winner: {0}", winnerName) : "We have no winner this time :(");
				eb.WithFooter("Free Premium giveaways - starts every Friday and Saturday!");
				eb.WithColor(Color.Red);
				var mes = await guild.GetTextChannel(channelId).SendMessageAsync("<:wolfRNG:607868292979490816> **GIVEAWAY ROULETTE!** <:wolfRNG:607868292979490816>", false, eb.Build());
				
				var db_param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "DiscordGiveawayPostId");
				if (db_param.Count < 1)
				{
					var new_param = new DBGlobalParams{
						Name = "DiscordGiveawayPostId",
						Value = mes.Id
					};
					DBSessions.SessionSave(new_param);
				}
				else
				{
					var postId = db_param.First();
					postId.Value = mes.Id;
					DBSessions.SessionUpdate(postId);
				}
			}
			catch (Exception e)
			{
				Logger.WarnException(e, "FinishGiveaway() exception: ");
			}
		}
    }
}
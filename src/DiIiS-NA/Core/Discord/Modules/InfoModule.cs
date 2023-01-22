
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiIiS_NA.Core.Extensions;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using DiIiS_NA.LoginServer.Battle;
using DiIiS_NA.LoginServer.GamesSystem;

namespace DiIiS_NA.Core.Discord.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
		private ulong AnnounceChannelId
		{
			get
			{
				return (ulong)DiIiS_NA.Core.Discord.Config.Instance.AnnounceChannelId;
			}
			set{}
		}
		
		private ulong StatsChannelId
		{
			get
			{
				return (ulong)DiIiS_NA.Core.Discord.Config.Instance.StatsChannelId;
			}
			set{}
		}
		
        [Command("about")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Info()
		{
            await ReplyAsync($"Hello, I am a bot called {Context.Client.CurrentUser.Username} written for Blizzless Server\nSpecial Thanks to those who want it.");
		}
				
		[Command("ping")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PingAsync()
		{
            await ReplyAsync("pong!");
		}
			
		[Command("list_online")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ListOnline()
		{
			int ccu = PlayerManager.OnlinePlayers.Count;
			string players = "";
			foreach (var plr in PlayerManager.OnlinePlayers)
			{
				players += plr.Account.BattleTag;
				players += "\n";
			}
			
			await ReplyAsync(string.Format("Total players online: {0}\n{1}", ccu, players));
		}
		
		[Command("lock")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Lock()
		{
			var param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "ServerOpened").First();
			param.Value = 0;
			DBSessions.SessionUpdate(param);
			await ReplyAsync("Login availability now **DISABLED**");
		}
		
		[Command("unlock")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Unlock()
		{
			var param = DBSessions.SessionQueryWhere<DBGlobalParams>(dbgp => dbgp.Name == "ServerOpened").First();
			param.Value = 1;
			DBSessions.SessionUpdate(param);
			await ReplyAsync("Login availability now **ENABLED**");
		}
		
		[Command("maintenance")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task MaintenanceAnnounce([Remainder]int minutes)
		{
			//notify players maintenance is enabled
			var messages = await Context.Guild.GetTextChannel(AnnounceChannelId).GetMessagesAsync(10).FlattenAsync();
			await Context.Guild.GetTextChannel(AnnounceChannelId).DeleteMessagesAsync(messages);
			await Context.Guild.GetTextChannel(AnnounceChannelId).SendMessageAsync("Servers status: :tools: **PLANNED MAINTENANCE**.");
            await Context.Guild.GetTextChannel(AnnounceChannelId).SendMessageAsync($"@here Servers will be restarted in **{minutes}** minutes for a planned maintenance.");
		}
		
		[Command("online")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task OnlineAnnounce()
		{
			var messages = await Context.Guild.GetTextChannel(AnnounceChannelId).GetMessagesAsync(10).FlattenAsync();
			await Context.Guild.GetTextChannel(AnnounceChannelId).DeleteMessagesAsync(messages);
			await Context.Guild.GetTextChannel(AnnounceChannelId).SendMessageAsync("Servers status: :white_check_mark: **ONLINE**.");
            await Context.Guild.GetTextChannel(AnnounceChannelId).SendMessageAsync("@here Servers are up and running.\n");
		}

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;

namespace DiIiS_NA.Core.Discord.Modules
{
    public class EventsModule : ModuleBase<SocketCommandContext>
    {	
		private ulong EventsChannelId
		{
			get
			{
				return (ulong)DiIiS_NA.Core.Discord.Config.Instance.EventsChannelId;
			}
			set{}
		}
		
		
		[Command("announce_event")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SalesAnnounce()
		{
			var eb = new EmbedBuilder();
			eb.WithTitle("New Event");
			eb.WithDescription("Event Description.");
			eb.WithFooter("Ends 4th Dec 2022.\nStay at home!");
			eb.WithColor(Color.Green);
			await Context.Guild.GetTextChannel(EventsChannelId).SendMessageAsync("<:party:> **NEW EVENT!** <:party:>", false, eb.Build());
		}

    }
}

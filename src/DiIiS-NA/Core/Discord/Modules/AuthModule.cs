
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.Core.Storage;
using DiIiS_NA.Core.Storage.AccountDataBase.Entities;
using Discord;
using Discord.Commands;

namespace DiIiS_NA.Core.Discord.Modules
{
    public class AuthModule : ModuleBase<SocketCommandContext>
    {
		[Command("register")]
		public async Task Register([Remainder] string args)
		{
			string dtag = Context.User.Username + "#" + Context.User.Discriminator;
			string[] registerInfo = args.Split(null);

			var email = registerInfo[0];
			var password = registerInfo[1];
			var battleTagName = registerInfo[2];
			var userLevel = Account.UserLevels.User;

			if (!(Context.Channel is IDMChannel))
			{
				await Context.Guild.GetTextChannel(Context.Channel.Id).DeleteMessageAsync(Context.Message);
				await ReplyAsync($"<@{Context.User.Id}> that command could be used only via direct message!\nDon't show your e-mail to anyone, don't be a fool! <:200iq:538833204421984286>");
				return;
			}


			if (registerInfo.Count() == 3)
			{
				if (!email.Contains('@'))
				{
					await ReplyAsync($"<@{Context.User.Id}> " + string.Format("'{0}' is not a valid email address.", email));
					return;
				}
				if (!IsValid(email))
				{
					await ReplyAsync("Your e-mail address is invalid!");
					return;
				}
				if (battleTagName.Contains('#'))
				{
					await ReplyAsync($"<@{Context.User.Id}> BattleTag must not contain '#' or HashCode.");
					return;
				}


				if (password.Length < 8 || password.Length > 16)
				{
					await ReplyAsync($"<@{Context.User.Id}> Password should be a minimum of 8 and a maximum of 16 characters.");
					return;
				}

				if (AccountManager.GetAccountByEmail(email) != null)
				{
					await ReplyAsync($"<@{Context.User.Id}> " + string.Format("An account already exists for email address {0}.", email));
					return;
				}

				var account = AccountManager.CreateAccount(email, password, battleTagName, userLevel);
				var gameAccount = GameAccountManager.CreateGameAccount(account);

				var guild_user = Context.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetUser(Context.User.Id);

				if (guild_user == null)
				{
					await ReplyAsync("Your Discord account is not participated channel!");
					return;
				}

				if (AccountManager.BindDiscordAccount(email, Context.User.Id, dtag))
				{
					var accountcheck = AccountManager.GetAccountByEmail(email);
					string battle_tag = account.DBAccount.BattleTagName + "#" + account.DBAccount.HashCode;
					await ReplyAsync($"Account registered.\nYour Discord account has been successfully bound to {battle_tag}!");
					await guild_user.AddRoleAsync(Context.Client.GetGuild((ulong)DiIiS_NA.Core.Discord.Config.Instance.GuildId).GetRole((ulong)DiIiS_NA.Core.Discord.Config.Instance.BaseRoleId));
					await ReplyAsync("You are now **DemonSlayer**!");
					try
					{
						await guild_user.ModifyAsync(x => { x.Nickname = battle_tag; });
					}
					catch 
					{ }
				}
				else
					await ReplyAsync("An error occured: make sure your Discord account hasn't already been bound to another account.!");
            }
            else
            {
				await ReplyAsync("Incorrect usage: !register <email> <password> <battletag>.!");
			}
		}
		
		private bool IsValid(string emailaddress)
		{
			try
			{
				MailAddress m = new MailAddress(emailaddress);

				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}
    }
}

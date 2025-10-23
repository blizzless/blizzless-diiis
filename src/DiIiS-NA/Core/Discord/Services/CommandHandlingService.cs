using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.Core.Discord.Services
{
	public class CommandHandlingService
	{
		private static readonly Logger Logger = LogManager.CreateLogger();
		private readonly CommandService _commands;
		private readonly DiscordSocketClient _discord;
		private readonly IServiceProvider _services;
	
		public CommandHandlingService(IServiceProvider services)
		{
			_commands = services.GetService<CommandService>();
			_discord = services.GetService<DiscordSocketClient>();
			_services = services;
	
			// Hook CommandExecuted to handle post-command-execution logic.
			_commands.CommandExecuted += CommandExecutedAsync;
			// Hook MessageReceived so we can process each message to see
			// if it qualifies as a command.
			_discord.MessageReceived += MessageReceivedAsync;
		}
	
		public async Task InitializeAsync()
		{
			// Register modules that are public and inherit ModuleBase<T>.
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
		}
	
		public async Task MessageReceivedAsync(SocketMessage rawMessage)
		{
			try
			{
				// Ignore system messages, or messages from other bots
				if (!(rawMessage is SocketUserMessage message)) return;
				if (message.Source != MessageSource.User) return;
				
				//if (!(message.Channel is SocketTextChannel textChannel)) return;
				//if (textChannel.Name != DiIiS_NA.Core.Discord.Config.Instance.ConsoleChannel) return;
				// This value holds the offset where the prefix ends
				var argPos = 0;
				// Perform prefix check. You may want to replace this with
				if (!message.HasCharPrefix('!', ref argPos)) return;
				// for a more traditional command format like !help.
				//if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;
				
				var context = new SocketCommandContext(_discord, message);
				// Perform the execution of the command. In this method,
				// the command service will perform precondition and parsing check
				// then execute the command if one is matched.
				Logger.BotCommand("[{0}]: {1}", message.Author.Username, message.Content);
				await _commands.ExecuteAsync(context, argPos, _services); 
				// Note that normally a result will be returned by this format, but here
				// we will handle the result in CommandExecutedAsync,
			}
			catch (Exception e)
			{
				Logger.DebugException(e, "Discord exception: ");
			}
		}
	
		public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
		{
			// command is unspecified when there was a search failure (command not found); we don't care about these errors
			if (!command.IsSpecified)
				return;
	
			// the command was successful, we don't care about this result, unless we want to log that a command succeeded.
			if (result.IsSuccess)
				return;
	
			// the command failed, let's notify the user that something happened.
			await context.Channel.SendMessageAsync($"error: {result}");
		}
	}
}

//Blizzless Project 2022 
using DiIiS_NA.Core.Logging;
//Blizzless Project 2022 
using DiIiS_NA.LoginServer.Battle;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Reflection;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public class CommandGroup
	{
		private static readonly Logger Logger = LogManager.CreateLogger("CM");

		public CommandGroupAttribute Attributes { get; private set; }

		private readonly Dictionary<CommandAttribute, MethodInfo> _commands =
			new Dictionary<CommandAttribute, MethodInfo>();

		public void Register(CommandGroupAttribute attributes)
		{
			this.Attributes = attributes;
			this.RegisterDefaultCommand();
			this.RegisterCommands();
		}

		private void RegisterCommands()
		{
			foreach (var method in this.GetType().GetMethods())
			{
				object[] attributes = method.GetCustomAttributes(typeof(CommandAttribute), true);
				if (attributes.Length == 0) continue;

				var attribute = (CommandAttribute)attributes[0];
				if (attribute is DefaultCommand) continue;

				if (!this._commands.ContainsKey(attribute))
					this._commands.Add(attribute, method);
				else
					Logger.Warn("There exists an already registered command '{0}'.", attribute.Name);
			}
		}

		private void RegisterDefaultCommand()
		{
			foreach (var method in this.GetType().GetMethods())
			{
				object[] attributes = method.GetCustomAttributes(typeof(DefaultCommand), true);
				if (attributes.Length == 0) continue;
				if (method.Name.ToLower() == "fallback") continue;

				this._commands.Add(new DefaultCommand(this.Attributes.MinUserLevel), method);
				return;
			}

			// set the fallback command if we couldn't find a defined DefaultCommand.
			this._commands.Add(new DefaultCommand(this.Attributes.MinUserLevel), this.GetType().GetMethod("Fallback"));
		}

		public virtual string Handle(string parameters, BattleClient invokerClient = null)
		{
			// check if the user has enough privileges to access command group.
			// check if the user has enough privileges to invoke the command.
			if (invokerClient != null && this.Attributes.MinUserLevel > invokerClient.Account.UserLevel)
				return "You don't have enough privileges to invoke that command.";

			string[] @params = null;
			CommandAttribute target = null;

			if (parameters == string.Empty)
				target = this.GetDefaultSubcommand();
			else
			{
				@params = parameters.Split(' ');
				target = this.GetSubcommand(@params[0]) ?? this.GetDefaultSubcommand();

				if (target != this.GetDefaultSubcommand())
					@params = @params.Skip(1).ToArray();
			}

			// check if the user has enough privileges to invoke the command.
			if (invokerClient != null && target.MinUserLevel > invokerClient.Account.UserLevel)
				return "You don't have enough privileges to invoke that command.";

			return (string)this._commands[target].Invoke(this, new object[] { @params, invokerClient });
		}

		public string GetHelp(string command)
		{
			foreach (var pair in this._commands)
			{
				if (command != pair.Key.Name) continue;
				return pair.Key.Help;
			}

			return string.Empty;
		}

		[DefaultCommand]
		public virtual string Fallback(string[] @params = null, BattleClient invokerClient = null)
		{
			var output = "Available subcommands: ";
			foreach (var pair in this._commands)
			{
				if (pair.Key.Name.Trim() == string.Empty) continue; // skip fallback command.
				if (invokerClient != null && pair.Key.MinUserLevel > invokerClient.Account.UserLevel) continue;
				output += pair.Key.Name + ", ";
			}

			return output.Substring(0, output.Length - 2) + ".";
		}

		protected CommandAttribute GetDefaultSubcommand()
		{
			return this._commands.Keys.First();
		}

		protected CommandAttribute GetSubcommand(string name)
		{
			return this._commands.Keys.FirstOrDefault(command => command.Name == name);
		}
	}
}

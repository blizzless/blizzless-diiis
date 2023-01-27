using DiIiS_NA.Core.Logging;
using DiIiS_NA.LoginServer.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	public class CommandGroup
	{
		private static readonly Logger Logger = LogManager.CreateLogger("CM");

		public CommandGroupAttribute Attributes { get; private set; }

		private readonly Dictionary<CommandAttribute, MethodInfo> _commands = new();

		public void Register(CommandGroupAttribute attributes)
		{
			Attributes = attributes;
			RegisterDefaultCommand();
			RegisterCommands();
		}

		private void RegisterCommands()
		{
			foreach (var method in GetType().GetMethods())
			{
				object[] attributes = method.GetCustomAttributes(typeof(CommandAttribute), true);
				if (attributes.Length == 0) continue;

				var attribute = (CommandAttribute)attributes[0];
				if (attribute is DefaultCommand) continue;

				if (!_commands.ContainsKey(attribute))
					_commands.Add(attribute, method);
				else
					Logger.Error("Command '{0}' already exists.", attribute.Name);
			}
		}

		private void RegisterDefaultCommand()
		{
			foreach (var method in GetType().GetMethods())
			{
				object[] attributes = method.GetCustomAttributes(typeof(DefaultCommand), true);
				if (attributes.Length == 0) continue;
				if (method.Name.ToLower() == "fallback") continue;

				_commands.Add(new DefaultCommand(Attributes.MinUserLevel), method);
				return;
			}

			// set the fallback command if we couldn't find a defined DefaultCommand.
			_commands.Add(new DefaultCommand(Attributes.MinUserLevel), GetType().GetMethod("Fallback"));
		}

		public virtual string Handle(string parameters, BattleClient invokerClient = null)
		{
			// check if the user has enough privileges to access command group.
			// check if the user has enough privileges to invoke the command.
			if (invokerClient != null && Attributes.MinUserLevel > invokerClient.Account.UserLevel)
				return "You don't have enough privileges to invoke that command.";

			string[] @params = null;
			CommandAttribute target = null;

			if (parameters == string.Empty)
				target = GetDefaultSubcommand();
			else
			{
				@params = parameters.Split(' ');
				target = GetSubcommand(@params[0]) ?? GetDefaultSubcommand();

				if (target != GetDefaultSubcommand())
					@params = @params.Skip(1).ToArray();
			}

			// check if the user has enough privileges to invoke the command.
			if (invokerClient != null && target.MinUserLevel > invokerClient.Account.UserLevel)
				return "You don't have enough privileges to invoke that command.";

			return (string)_commands[target].Invoke(this, new object[] { @params, invokerClient });
		}

		public string GetHelp(string command)
		{
			foreach (var pair in _commands)
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
			foreach (var pair in _commands)
			{
				if (pair.Key.Name.Trim() == string.Empty) continue; // skip fallback command.
				if (invokerClient != null && pair.Key.MinUserLevel > invokerClient.Account.UserLevel) continue;
				output += pair.Key.Name + ", ";
			}

			return output.Substring(0, output.Length - 2) + ".";
		}

		protected CommandAttribute GetDefaultSubcommand()
		{
			return _commands.Keys.First();
		}

		protected CommandAttribute GetSubcommand(string name)
		{
			return _commands.Keys.FirstOrDefault(command => command.Name == name);
		}
	}
}

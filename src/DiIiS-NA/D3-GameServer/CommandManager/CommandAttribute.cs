using DiIiS_NA.LoginServer.AccountsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.CommandManager
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CommandGroupAttribute : Attribute
	{
		/// <summary>
		/// Command group's name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Help text for command group.
		/// </summary>
		public string Help { get; private set; }

		/// <summary>
		/// Minimum user level required to invoke the command.
		/// </summary>
		public Account.UserLevels MinUserLevel { get; private set; }

		public CommandGroupAttribute(string name, string help, Account.UserLevels minUserLevel = Account.UserLevels.Admin)
		{
			Name = name.ToLower();
			Help = help;
			MinUserLevel = minUserLevel;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		/// <summary>
		/// Command's name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Help text for command.
		/// </summary>
		public string Help { get; private set; }

		/// <summary>
		/// Minimum user level required to invoke the command.
		/// </summary>
		public Account.UserLevels MinUserLevel { get; private set; }

		public CommandAttribute(string command, string help, Account.UserLevels minUserLevel = Account.UserLevels.User)
		{
			Name = command.ToLower();
			Help = help;
			MinUserLevel = minUserLevel;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class DefaultCommand : CommandAttribute
	{
		public DefaultCommand(Account.UserLevels minUserLevel = Account.UserLevels.User)
			: base("", "", minUserLevel)
		{
		}
	}
}

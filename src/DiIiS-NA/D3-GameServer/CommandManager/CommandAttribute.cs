//Blizzless Project 2022 
using DiIiS_NA.LoginServer.AccountsSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
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
			this.Name = name.ToLower();
			this.Help = help;
			this.MinUserLevel = minUserLevel;
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
			this.Name = command.ToLower();
			this.Help = help;
			this.MinUserLevel = minUserLevel;
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

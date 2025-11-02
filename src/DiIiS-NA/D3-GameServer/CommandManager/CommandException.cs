using System;
using DiIiS_NA.LoginServer.AccountsSystem;
using DiIiS_NA.Utilities;

namespace DiIiS_NA.GameServer.CommandManager;

public class CommandException : Exception
{
    public CommandException(string message) : base(message) {}
    public CommandException(string message, Exception ex) : base(message, ex) {}
}

public class NotEnoughPrivilegeException : CommandException
{
    public NotEnoughPrivilegeException() : base("You do not have enough privileges to execute this command.") { }
    public NotEnoughPrivilegeException(Account.UserLevels userLevel) : base($"You do not have enough privileges to execute this command (must be {userLevel.GetName()}).") { }
}
public class InGameOnlyException : CommandException
{
    public InGameOnlyException() : base("You can only execute this command whilst in-game.") { }
}
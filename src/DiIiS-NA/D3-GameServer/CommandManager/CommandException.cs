using System;

namespace DiIiS_NA.GameServer.CommandManager;

public class CommandException : Exception
{
    public CommandException(string message) : base(message) {}
    public CommandException(string message, Exception ex) : base(message, ex) {}
}
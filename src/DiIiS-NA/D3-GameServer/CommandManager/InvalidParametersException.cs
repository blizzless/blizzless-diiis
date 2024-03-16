using System;

namespace DiIiS_NA.GameServer.CommandManager;

public class InvalidParametersException : CommandException
{
    public InvalidParametersException(string message) : base(message) {}
    public InvalidParametersException(string message, Exception ex) : base(message, ex) {}
}
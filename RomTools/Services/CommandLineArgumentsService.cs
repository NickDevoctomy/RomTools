using System.Reflection;

namespace RomTools.Services;

public class CommandLineArgumentsService : ICommandLineArgumentService
{
    public string GetArguments(string fullCommandLine)
    {
        var curExePath = Assembly.GetEntryAssembly().Location;
        var arguments = fullCommandLine.Replace(curExePath, string.Empty).Trim();
        return arguments;
    }
}

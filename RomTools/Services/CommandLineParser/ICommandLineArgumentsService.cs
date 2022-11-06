namespace RomTools.Services.CommandLineParser;

public interface ICommandLineArgumentService
{
    string GetArguments(string fullCommandLine);
}
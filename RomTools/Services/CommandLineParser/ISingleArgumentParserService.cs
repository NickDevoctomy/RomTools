namespace RomTools.Services.CommandLineParser;

public interface ISingleArgumentParserService
{
    Argument Parse(string argumentString);
}

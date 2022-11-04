using System;

namespace RomTools.Services.CommandLineParser;

public interface ICommandLineParserService
{
    bool TryParseArgumentsAsOptions<T>(string argumentString, out ParseResults options);
    bool TryParseArgumentsAsOptions(Type optionsType, string argumentString, out ParseResults options);
}

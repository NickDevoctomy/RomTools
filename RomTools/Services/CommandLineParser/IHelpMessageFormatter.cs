using System;

namespace RomTools.Services.CommandLineParser;

public interface IHelpMessageFormatter
{
    string Format<T>();
    string Format(Type optionsType);
}

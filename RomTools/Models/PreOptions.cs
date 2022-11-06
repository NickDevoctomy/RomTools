using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Models;

[ExcludeFromCodeCoverage]
public class PreOptions
{
    [CommandLineParserOption(
        Required = true,
        ShortName = "c",
        LongName = "command",
        IsDefault = true,
        DisplayName = "Command",
        HelpText = "Command to perform")]
    public Command Command { get; set; }
}

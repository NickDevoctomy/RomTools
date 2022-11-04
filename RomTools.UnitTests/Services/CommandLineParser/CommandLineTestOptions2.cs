using RomTools.Services.CommandLineParser;

namespace RomTools.UnitTests.Services.CommandLineParser;

public class CommandLineTestOptions2
{

    [CommandLineParserOption(
        LongName = "enum",
        ShortName = "e",
        Required = false,
        IsDefault = true,
        DisplayName = "Enum",
        HelpText = "Some enum value")]
    public TestEnum EnumValue { get; set; }
}
using RomTools.Services.CommandLineParser;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateHashedCollectionOptions : PreOptions
    {
        [CommandLineParserOption(HelpText = "Path containing the rom set", Required = true, ShortName = "p", LongName = "path")]
        public string Path { get; set; }

        [CommandLineParserOption(HelpText = "Verbose logging", Required = false, ShortName = "v", LongName = "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}

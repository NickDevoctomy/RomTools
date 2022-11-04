using RomTools.Services.CommandLineParser;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Services
{
    [ExcludeFromCodeCoverage]
    public class PruneRomsOptions : PreOptions
    {
        [CommandLineParserOption(Required = true, ShortName = "p", LongName = "path")]
        public string Path { get; set; }

        [CommandLineParserOption(Required = false, ShortName = "v", LongName = "verbose", DefaultValue = true)]
        public bool Verbose { get; set; }
    }
}

using RomTools.Services.CommandLineParser;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Models
{
    [ExcludeFromCodeCoverage]
    public class PathCompareOptions : PreOptions
    {
        [CommandLineParserOption(HelpText = "Path containing the first (a) rom set to compare", Required = true, ShortName = "a", LongName = "patha")]
        public string PathA { get; set; }

        [CommandLineParserOption(HelpText = "Path containing the second (b) rom set to compare", Required = true, ShortName = "b", LongName = "pathb")]
        public string PathB { get; set; }

        [CommandLineParserOption(HelpText = "Verbose logging", Required = false, ShortName = "v", LongName = "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}

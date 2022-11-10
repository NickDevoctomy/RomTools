using RomTools.Services.CommandLineParser;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Models
{
    [ExcludeFromCodeCoverage]
    public class PruneRomsOptions : PreOptions
    {
        [CommandLineParserOption(HelpText = "Path containing the rom set", Required = true, ShortName = "p", LongName = "path")]
        public string Path { get; set; }

        [CommandLineParserOption(HelpText = "Prioritise these languages, discard everything else", Required = false, ShortName = "l", LongName = "languages", DefaultValue = "en")]
        public string Languages { get; set; }

        [CommandLineParserOption(HelpText = "Only include verified dumps", Required = false, ShortName = "d", LongName = "verified", DefaultValue = false)]
        public bool Verified { get; set; }

        [CommandLineParserOption(HelpText = "Hash the files to determine binary duplicates", Required = false, ShortName = "h", LongName = "hash", DefaultValue = false)]
        public bool HashFiles { get; set; }

        [CommandLineParserOption(HelpText = "Report what changes will be made.  This will not actually delete anything.", Required = false, ShortName = "r", LongName = "report", DefaultValue = false)]
        public bool Report { get; set; }

        [CommandLineParserOption(HelpText = "Verbose logging", Required = false, ShortName = "v", LongName = "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}

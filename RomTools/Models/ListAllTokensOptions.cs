using RomTools.Services.CommandLineParser;

namespace RomTools.Models
{
    public class ListAllTokensOptions : PreOptions
    {
        [CommandLineParserOption(HelpText = "Path containing the rom set", Required = true, ShortName = "p", LongName = "path")]
        public string Path { get; set; }

        [CommandLineParserOption(HelpText = "Verbose logging", Required = false, ShortName = "v", LongName = "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}

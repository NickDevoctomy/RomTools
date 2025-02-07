﻿using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;
using System.Diagnostics.CodeAnalysis;

namespace RomTools.Models
{
    [ExcludeFromCodeCoverage]
    public class CreateHashedCollectionOptions : PreOptions
    {
        [CommandLineParserOption(HelpText = "Path containing the rom set", Required = true, ShortName = "p", LongName = "path")]
        public string Path { get; set; }

        [CommandLineParserOption(HelpText = "Path to the output json file to create", Required = true, ShortName = "o", LongName = "output")]
        public string Output { get; set; }

        [CommandLineParserOption(HelpText = "Name of the rom set collection", Required = false, ShortName = "n", LongName = "name", DefaultValue = "NewCollection1")]
        public string Name { get; set; }

        [CommandLineParserOption(HelpText = "Description of the rom set collection", Required = false, ShortName = "d", LongName = "description", DefaultValue = "New rom set collection")]
        public string Description { get; set; }

        [CommandLineParserOption(HelpText = "Algorithm to use for generating the hashes", Required = false, ShortName = "a", LongName = "alg", DefaultValue = HashingAlgorithm.Md5)]
        public HashingAlgorithm Algorithm { get; set; }

        [CommandLineParserOption(HelpText = "Verbose logging", Required = false, ShortName = "v", LongName = "verbose", DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}

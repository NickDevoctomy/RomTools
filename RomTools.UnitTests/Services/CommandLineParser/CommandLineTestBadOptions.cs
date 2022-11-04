﻿using RomTools.Services.CommandLineParser;
using System;

namespace RomTools.UnitTests.Services.CommandLineParser;

public class CommandLineTestBadOptions
{
    [CommandLineParserOption(LongName = "Unsupported", ShortName = "u", Required = true, IsDefault = true)]
    public Guid UnsupportedValue { get; set; }
}
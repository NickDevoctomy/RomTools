﻿using RomTools.Services;
using RomTools.Services.CommandLineParser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using RomTools.Extensions;
using Newtonsoft.Json;
using RomTools.Services.Commands;

namespace RomTools;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static Config Config { get; private set; }

    static async Task<int> Main(string[] arguments)
    {
        using IHost host = CreateHostBuilder(null).Build();

        var configRaw = await File.ReadAllTextAsync("config/config.json");
        Config = JsonConvert.DeserializeObject<Config>(configRaw);

        var program = host.Services.GetService<IProgram>();
        return await program.Run(arguments, CancellationToken.None);       // Somehow we should bind a hot key to use for cancellation token
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) => services
        .AddSingleton<ICommandLineArgumentService, CommandLineArgumentsService>()
        .AddSingleton<IArgumentsFlattenerService, ArgumentsFlattenerService>()
        .AddSingleton<ICommandLineParserService, CommandLineParserService>((IServiceProvider _) => { return CommandLineParserService.CreateDefaultInstance(); })
        .AddTransient<IHelpMessageFormatter, HelpMessageFormatter>()
        .AddTransient<IMd5HasherService, Md5HasherService>()
        .AddTransient<ITokenExtractorService, TokenExtractorService>()
        .AddFileFilters()
        .AddCommands()
        .AddSingleton<IProgram, RomToolsProgram>());


}

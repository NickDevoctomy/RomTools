using Newtonsoft.Json.Linq;
using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;

namespace RomTools.Services.Commands;

public class ListAllTokenSetsService : IListAllTokenSetsService, ICommand
{
    private readonly ICommandLineParserService _commandLineParserService;
    private readonly ITokenExtractorService _tokenExtractorService;

    public ListAllTokensOptions _options;

    public ListAllTokenSetsService(
        ICommandLineParserService commandLineParserService,
        ITokenExtractorService tokenExtractorService)
    {
        _commandLineParserService = commandLineParserService;
        _tokenExtractorService = tokenExtractorService;
    }

    public async Task<int> ListAsync(
        ListAllTokensOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        _options = options;

        LogMessage($"Getting all files from path '{options.Path}'.", false, ConsoleColor.White, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);

        LogMessage($"Getting all tokens from file names.", false, ConsoleColor.White, options);
        var roundTokens = GetAllTokensSets(sourceFiles, "(", ")");
        var squareTokens = GetAllTokensSets(sourceFiles, "[", "]");

        LogMessage($"The following {roundTokens.Count} round braced token sets were found,", false, ConsoleColor.White, options);
        roundTokens.ForEach(x => LogMessage(x, false, ConsoleColor.Green, options));

        LogMessage($"The following {squareTokens.Count} square braced token sets were found,", false, ConsoleColor.White, options);
        squareTokens.ForEach(x => LogMessage(x, false, ConsoleColor.Green, options));

        return (int)ReturnCodes.Success;
    }

    private List<string> GetAllTokensSets(
        List<FileEnvelope> files,
        params string[] braces)
    {
        var tokens = new List<string>();
        files.ForEach(x =>
        {
            var curSets = _tokenExtractorService.ExtractTokens(x, braces);
            curSets = curSets.OrderBy(x => x).ToList();
            var set = string.Join(':', curSets);
            if (!tokens.Contains(set) && !string.IsNullOrWhiteSpace(set))
            {
                tokens.Add(set);
            }
        });

        return tokens;
    }

    public bool IsApplicable(PreOptions preOptions)
    {
        return preOptions.Command == Command.ListAllTokenSets;
    }

    public async Task<int> RunAsync(
        string arguments,
        CancellationToken cancellationToken)
    {
        if (_commandLineParserService.TryParseArgumentsAsOptions(
            typeof(ListAllTokensOptions),
            arguments,
            out var listAllTokensOptions))
        {
            return await ListAsync(
                (ListAllTokensOptions)listAllTokensOptions.Options,
                cancellationToken);
        }
        else
        {
            Console.WriteLine($"{listAllTokensOptions.Exception.Message}");
        }

        return (int)ReturnCodes.Unknown;
    }

    private static List<FileEnvelope> GetAllFilesFromPath(string path)
    {
        var allFiles = Directory.GetFiles(path, "*.*").ToList();
        return allFiles.Select(x => new FileEnvelope(x)).ToList();
    }

    private static void LogMessage(
        string message,
        bool isVerbose,
        ConsoleColor color,
        ListAllTokensOptions options)
    {
        if (!isVerbose || isVerbose && options.Verbose)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = prevColor;
        }
    }
}

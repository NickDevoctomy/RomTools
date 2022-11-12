using Newtonsoft.Json.Linq;
using RomTools.Models;
using RomTools.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RomTools.Services.Commands;

public class ListAllTokenSetsService : IListAllTokenSetsService
{
    private readonly ITokenExtractorService _tokenExtractorService;

    public ListAllTokensOptions _options;

    public ListAllTokenSetsService(ITokenExtractorService tokenExtractorService)
    {
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

    private static List<FileEnvelope> GetAllFilesFromPath(string path)
    {
        var allFiles = Directory.GetFiles(path, "*.*").ToList();
        return allFiles.Select(x => new FileEnvelope(x)).ToList();
    }

    private void LogAction(
        string message,
        bool isVerbose)
    {
        LogMessage(
            $"{DateTime.Now} :: {message}",
            isVerbose,
            ConsoleColor.White,
            _options);
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

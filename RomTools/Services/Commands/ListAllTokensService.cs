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

public class ListAllTokensService : IListAllTokensService
{
    public ListAllTokensOptions _options;

    public async Task<int> ListAsync(
        ListAllTokensOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        _options = options;

        LogMessage($"Getting all files from path '{options.Path}'.", false, ConsoleColor.White, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);

        LogMessage($"Getting all tokens from file names.", false, ConsoleColor.White, options);
        var roundTokens = GetAllTokens(sourceFiles, "(", ")");
        var squareTokens = GetAllTokens(sourceFiles, "[", "]");

        LogMessage($"The following {roundTokens.Count} round braced tokens were found,", false, ConsoleColor.White, options);
        roundTokens.ForEach(x => LogMessage(x, false, ConsoleColor.Green, options));

        LogMessage($"The following {squareTokens.Count} square braced tokens were found,", false, ConsoleColor.White, options);
        squareTokens.ForEach(x => LogMessage(x, false, ConsoleColor.Green, options));

        return (int)ReturnCodes.Success;
    }

    private static List<string> GetAllTokens(
        List<FileEnvelope> files,
        params string[] braces)
    {
        var tokens = new List<string>();
        files.ForEach(x =>
        {
            var fileInfo = new FileInfo(x.FullPath);
            var matches = Regex.Matches(fileInfo.Name, $"[{braces[0]}][a-zA-Z0-9]+[{braces[1]}]");
            matches.ToList().ForEach(y =>
            {
                if (!tokens.Contains(y.Value))
                {
                    tokens.Add(y.Value);
                }
            });
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

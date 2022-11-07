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

public class ListAllLanguagesService : IListAllLanguagesService
{
    public ListAllLanguagesOptions _options;

    public async Task<int> ListAsync(
        ListAllLanguagesOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        _options = options;

        LogMessage($"Getting all files from path '{options.Path}'.", false, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);

        LogMessage($"Getting all languages from file names.", false, options);
        var langauges = GetAllLanguages(sourceFiles);

        LogMessage($"The following {langauges.Count} language tokens were found,", false, options);
        langauges.ForEach(x => LogMessage(x, false, options));

        return (int)ReturnCodes.Success;
    }

    private static List<string> GetAllLanguages(List<FileEnvelope> files)
    {
        var languages = new List<string>();
        files.ForEach(x =>
        {
            var fileInfo = new FileInfo(x.FullPath);
            var matches = Regex.Matches(fileInfo.Name, $"[(][a-zA-Z]+[)]");
            matches.ToList().ForEach(y =>
            {
                if (!languages.Contains(y.Value))
                {
                    languages.Add(y.Value);
                }
            });
        });

        return languages;
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
        LogMessage($"{DateTime.Now} :: {message}", isVerbose, _options);
    }

    private static void LogMessage(
        string message,
        bool isVerbose,
        ListAllLanguagesOptions options)
    {
        if (!isVerbose || isVerbose && options.Verbose)
        {
            Console.WriteLine(message);
        }
    }
}

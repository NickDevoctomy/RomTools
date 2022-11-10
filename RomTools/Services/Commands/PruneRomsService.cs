using RomTools.Models;
using RomTools.Services.Enums;
using RomTools.Services.FileFilters;

namespace RomTools.Services.Commands;

public class PruneRomsService : IPruneRomsService
{
    private readonly IEnumerable<IFileFilter> _fileFilters;
    private readonly IMd5HasherService _md5HasherService;

    private PruneRomsOptions _options;

    public PruneRomsService(
        IEnumerable<IFileFilter> fileFilters,
        IMd5HasherService md5HasherService)
    {
        _fileFilters = fileFilters;
        _md5HasherService = md5HasherService;
    }

    public async Task<int> ProcessAsync(
        PruneRomsOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        _options = options;

        LogMessage($"Getting all files from path '{options.Path}'.", false, ConsoleColor.White, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);
        var allFiles = sourceFiles.ToList();

        LogMessage($"Got {allFiles.Count} files.", false, ConsoleColor.White, options);

        if (options.HashFiles)
        {
            LogMessage($"Hashing all files.", false, ConsoleColor.White, options);
            _md5HasherService.HashAll(allFiles);
            LogMessage($"All files hashed.", false, ConsoleColor.White, options);
        }

        LogMessage($"Processing file filters", false, ConsoleColor.White, options);
        var filterOptions = new Dictionary<string, object>();
        filterOptions.Add("language", options.Languages);
        filterOptions.Add("verified", options.Verified);
        foreach (var curFileFilter in _fileFilters.Where(x => x.IsApplicable(options)))
        {
            LogMessage($"Processing file filer: {curFileFilter.Description}", true, ConsoleColor.White, options);
            allFiles = curFileFilter.Filter(allFiles, filterOptions, LogAction);
            LogMessage($"{allFiles.Count} Files remaining after filtering.", true, ConsoleColor.White, options);
        }
        LogMessage($"{allFiles.Count} Files remaining after processing all filters.", false, ConsoleColor.White, options);

        var filesToDelete = sourceFiles.Where(x => !allFiles.Contains(x)).ToList();
        if (filesToDelete.Count == 0)
        {
            LogMessage("Analysis complete, there is nothing to do!", false, ConsoleColor.White, options);
            return (int)ReturnCodes.Success;
        }

        if(options.Report)
        {
            LogMessage($"Analysis complete, {filesToDelete.Count} files suggested for deletion,", false, ConsoleColor.White, options);
            foreach (var curFile in sourceFiles)
            {
                var delete = filesToDelete.Contains(curFile);
                LogMessage(
                    $"{new FileInfo(curFile.FullPath).Name}",
                    false,
                    delete ? ConsoleColor.Red : ConsoleColor.Green,
                    options);
            }
        }
        else
        {
            LogMessage($"Analysis complete, the following {filesToDelete.Count} files will be deleted,", false, ConsoleColor.White, options);
            filesToDelete.ForEach(x => LogMessage(x.FullPath, false, ConsoleColor.Red, options));
            LogMessage($"There will be {sourceFiles.Count - filesToDelete.Count} remaining after deletion.", false, ConsoleColor.White, options);
            LogMessage("WARNING! This operation cannot be undone.", false, ConsoleColor.Red, options);
            LogMessage("Are you sure (y/n)?", false, ConsoleColor.White, options);
            var prompt = Console.ReadKey();
            if (prompt.Key == ConsoleKey.Y)
            {
                filesToDelete.ForEach(x => File.Delete(x.FullPath));
            }
        }


        return (int)ReturnCodes.Success;
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
        LogMessage($"{DateTime.Now} :: {message}", isVerbose, ConsoleColor.White, _options);
    }

    private static void LogMessage(
        string message,
        bool isVerbose,
        ConsoleColor color,
        PruneRomsOptions options)
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

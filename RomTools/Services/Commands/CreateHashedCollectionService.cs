using Newtonsoft.Json;
using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;
using System.Threading;

namespace RomTools.Services.Commands;

public class CreateHashedCollectionService : ICreateHashedCollectionService, ICommand
{
    private readonly ICommandLineParserService _commandLineParserService;
    private readonly IMd5HasherService _md5HasherService;

    public CreateHashedCollectionService(
        ICommandLineParserService commandLineParserService,
        IMd5HasherService md5HasherService)
    {
        _commandLineParserService = commandLineParserService;
        _md5HasherService = md5HasherService;
    }

    public async Task<int> CreateAsync(
        CreateHashedCollectionOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        LogMessage($"Getting all files from path '{options.Path}'.", false, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);
        var allFiles = sourceFiles.ToList();

        LogMessage($"Got {allFiles.Count} files.", false, options);

        LogMessage($"Hashing all files.", false, options);
        _md5HasherService.HashAll(allFiles, true);
        LogMessage($"All files hashed.", false, options);

        var collectionFiles = allFiles.Select(x => new
        {
            FileName = new FileInfo(x.FullPath).Name,
            Archived = x.Properties["Archived"],
            ArchivedRomName = x.Properties["ArchivedRomName"],
            ArchivedRomMd5Hash = x.Properties["ArchivedRomMd5Hash"],
            RawMd5Hash = x.Properties["RawMd5Hash"],
        });

        var collection = new
        {
            options.Name,
            options.Description,
            CreatedAt = DateTime.UtcNow,
            HashingAlgorithm = options.Algorithm.ToString(),
            Files = collectionFiles
        };

        LogMessage($"Generating collection json at '{options.Output}'.", false, options);
        var collectionJson = JsonConvert.SerializeObject(collection, Formatting.Indented);
        await File.WriteAllTextAsync(options.Output, collectionJson, cancellationToken);
        LogMessage($"Finished generating collection json at '{options.Output}'.", false, options);

        return (int)ReturnCodes.Success;
    }

    public bool IsApplicable(PreOptions preOptions)
    {
        return preOptions.Command == Command.CreateHashedCollection;
    }

    public async Task<int> RunAsync(
        string arguments,
        CancellationToken cancellationToken)
    {
        if (_commandLineParserService.TryParseArgumentsAsOptions(
            typeof(CreateHashedCollectionOptions),
            arguments,
            out var createHashedCollectionOptions))
        {
            return await CreateAsync(
                (CreateHashedCollectionOptions)createHashedCollectionOptions.Options,
                cancellationToken);
        }
        else
        {
            Console.WriteLine($"{createHashedCollectionOptions.Exception.Message}");
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
        CreateHashedCollectionOptions options)
    {
        if (!isVerbose || isVerbose && options.Verbose)
        {
            Console.WriteLine(message);
        }
    }
}

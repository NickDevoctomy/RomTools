using Newtonsoft.Json;
using RomTools.Models;
using RomTools.Services.Enums;

namespace RomTools.Services.Commands;

public class CreateHashedCollectionService : ICreateHashedCollectionService
{
    private readonly IMd5HasherService _md5HasherService;
    private CreateHashedCollectionOptions _options;

    public CreateHashedCollectionService(IMd5HasherService md5HasherService)
    {
        _md5HasherService = md5HasherService;
    }

    public async Task<int> CreateAsync(
        CreateHashedCollectionOptions options,
        CancellationToken cancellationToken)
    {
        await Task.Yield();
        _options = options;

        LogMessage($"Getting all files from path '{options.Path}'.", false, options);
        var sourceFiles = GetAllFilesFromPath(options.Path);
        var allFiles = sourceFiles.ToList();

        LogMessage($"Got {allFiles.Count} files.", false, options);

        LogMessage($"Hashing all files.", false, options);
        _md5HasherService.HashAll(allFiles);
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
        CreateHashedCollectionOptions options)
    {
        if (!isVerbose || isVerbose && options.Verbose)
        {
            Console.WriteLine(message);
        }
    }
}

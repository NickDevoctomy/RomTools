using RomTools.Models;
using RomTools.Services.Enums;
using RomTools.Services.FileFilters;

namespace RomTools.Services
{
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

            LogMessage($"Getting all files from path '{options.Path}'.", false, options);
            var sourceFiles = GetAllFilesFromPath(options.Path);
            var allFiles = sourceFiles.ToList();

            LogMessage($"Got {allFiles.Count} files.", false, options);

            LogMessage($"Hashing all files.", false, options);
            _md5HasherService.HashAll(allFiles);
            LogMessage($"All files hashed.", false, options);

            LogMessage($"Processing file filters", false, options);
            var filterOptions = new Dictionary<string, object>();
            filterOptions.Add("language", options.Languages);
            filterOptions.Add("verified", options.Verified);
            foreach (var curFileFilter in _fileFilters)
            {
                LogMessage($"Processing file filer: {curFileFilter.Description}", true, options);
                allFiles = curFileFilter.Filter(allFiles, filterOptions, LogAction);
                LogMessage($"{allFiles.Count} Files remaining after filtering.", true, options);
            }
            LogMessage($"{allFiles.Count} Files remaining after processing all filters.", false, options);

            var filesToDelete = sourceFiles.Where(x => !allFiles.Contains(x)).ToList();
            if(filesToDelete.Count == 0)
            {
                LogMessage("Analysis complete, there is nothing to do!", false, options);
                return (int)ReturnCodes.Success;
            }

            LogMessage("Analysis complete, the following files will be deleted,", false, options);
            filesToDelete.ForEach(x => LogMessage(x.FullPath, false, options));
            LogMessage("WARNING! This operation cannot be undone.", false, options);
            LogMessage("Are you sure (y/n)?", false, options);
            var prompt = Console.ReadKey();
            if(prompt.Key == ConsoleKey.Y)
            {
                filesToDelete.ForEach(x => File.Delete(x.FullPath));
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
            LogMessage($"{DateTime.Now} :: {message}", isVerbose, _options);
        }

        private static void LogMessage(
            string message,
            bool isVerbose,
            PruneRomsOptions options)
        {
            if(!isVerbose || (isVerbose && options.Verbose))
            {
                Console.WriteLine(message);
            }
        }
    }
}

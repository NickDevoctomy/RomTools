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

        public async Task<int> Process(PruneRomsOptions options)
        {
            await Task.Yield();
            _options = options;

            LogMessage($"Getting all files from path '{options.Path}'.", false, options);
            var allFiles = GetAllFilesFromPath(options.Path);
            LogMessage($"Got {allFiles.Count} files.", false, options);

            LogMessage($"Hashing all files.", false, options);
            _md5HasherService.HashAll(allFiles);
            LogMessage($"All files hashed.", false, options);

            LogMessage($"Processing file filters", false, options);
            foreach (var curFileFilter in _fileFilters)
            {
                LogMessage($"Processing file filer: {curFileFilter.Description}", true, options);
                allFiles = curFileFilter.Filter(allFiles, LogAction);
                LogMessage($"{allFiles.Count} Files remaining after filtering.", true, options);
            }
            LogMessage($"{allFiles.Count} Files remaining after processing all filters.", false, options);

            return -1;
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

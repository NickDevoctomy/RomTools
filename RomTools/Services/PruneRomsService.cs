using RomTools.Services.DuplicateFileFilters;

namespace RomTools.Services
{
    public class PruneRomsService : IPruneRomsService
    {
        private readonly IEnumerable<IFileFilter> _fileFilters;

        private PruneRomsOptions _options;

        public PruneRomsService(IEnumerable<IFileFilter> fileFilters)
        {
            _fileFilters = fileFilters;
        }

        public async Task<int> Process(PruneRomsOptions options)
        {
            await Task.Yield();
            _options = options;

            LogMessage($"Getting all files from path '{options.Path}'.", false, options);
            var allFiles = GetAllFilesFromPath(options.Path);
            LogMessage($"Got {allFiles.Count} files.", false, options);

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
            LogMessage(message, isVerbose, _options);
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

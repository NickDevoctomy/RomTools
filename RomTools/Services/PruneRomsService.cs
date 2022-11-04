namespace RomTools.Services
{
    public class PruneRomsService : IPruneRomsService
    {
        public async Task<int> Process(PruneRomsOptions options)
        {
            await Task.Yield();

            var allFiles = GetAllFilesFromPath(options.Path);
            var filteredByExtension = IncludeByExtension(allFiles);

            return -1;
        }

        private List<string> GetAllFilesFromPath(string path)
        {
            var allFiles = Directory.GetFiles(path, "*.*");
            return allFiles.ToList();
        }

        private List<string> IncludeByExtension(List<string> files)
        {
            return files
                .Where(x => !(new FileInfo(x).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
    }
}

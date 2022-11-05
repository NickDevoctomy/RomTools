using RomTools.Services.DuplicateFileFilters;

namespace RomTools.Services.FileFilters
{
    public class FileExtensionFilter : IFileFilter
    {
        public string Description => "Filter files by their file extension.";

        public List<FileEnvelope> Filter(
            List<FileEnvelope> files,
            Action<string, bool> log)
        {
            var filtered = files
                .Where(x => new FileInfo(x.FullPath).Extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            log("All files filtered by extension.", true);
            log(string.Empty, true);
            foreach (var curFiltered in filtered)
            {
                log(curFiltered.FullPath, true);
            }

            return files
                .Where(x => !filtered.Contains(x))
                .ToList();
        }
    }
}

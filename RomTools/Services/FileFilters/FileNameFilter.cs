using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RomTools.Services.FileFilters
{
    public class FileNameFilter : IFileFilter
    {
        public int Priority => 2;
        public string Description => "Filter files by their file names.";

        public List<FileEnvelope> Filter(
            List<FileEnvelope> files,
            Action<string, bool> log)
        {
            log($"Files before filtering ({files.Count}):", true);
            files.ForEach(x => log(x.FullPath, true));

            var groupedBySimilarNames = GroupBySimilarNames(files);

            // Only english
            var filteredByLanguage = groupedBySimilarNames
                .Where(x => x.Value.Count > 0)
                .Select(x => GetMostSuitable(x.Value, false, "(u)", "(w)", "(e)", "(ue)"))
                .ToList();

            // Only verified
            filteredByLanguage = filteredByLanguage
                .Select(x => GetMostSuitable(x, true, "[!]"))
                .ToList();

            var postFiltered = filteredByLanguage.SelectMany(x => x).ToList();

            log($"Files after filtering ({postFiltered.Count}):", true);
            postFiltered.ForEach(x => log(x.FullPath, true));

            return postFiltered;
        }

        private Dictionary<string, List<FileEnvelope>> GroupBySimilarNames(List<FileEnvelope> files)
        {
            var grouped = new Dictionary<string, List<FileEnvelope>>();
            foreach (var curFile in files)
            {
                var truncatedFileName = TruncateFileNameUptoFirst(curFile.FullPath, '(', '[', '.');
                if(!grouped.ContainsKey(truncatedFileName))
                {
                    grouped.Add(truncatedFileName, new List<FileEnvelope>());
                }

                grouped[truncatedFileName].Add(curFile);
            }

            return grouped;
        }

        private string TruncateFileNameUptoFirst(
            string fullPath,
            params char[] delimiter)
        {
            var name = new FileInfo(fullPath).Name;
            var orderedDelimiters = delimiter.ToDictionary(a => a, b => name.IndexOf(b)).OrderBy(c => c.Value).ToList();
            if(!orderedDelimiters.Any(x => x.Value > 0))
            {
                return name;
            }
            
            var firstDelimiter = orderedDelimiters.FirstOrDefault(x => x.Value > 0);
            return name.Substring(0, name.IndexOf(firstDelimiter.Key));
        }

        private static List<FileEnvelope> GetMostSuitable(
            List<FileEnvelope> duplicates,
            bool allowNone,
            params string[] priorityTokens)
        {
            if (!allowNone && duplicates.Count == 1)
            {
                return new List<FileEnvelope>
                {
                    duplicates[0]
                };
            }

            foreach (var curToken in priorityTokens)
            {
                if (duplicates.Any(x => x.FullPath.Contains(curToken, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return duplicates.Where(x => x.FullPath.Contains(curToken, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
            }

            return new List<FileEnvelope>();
        }
    }
}

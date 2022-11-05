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
            var groupedBySimilarNames = GroupBySimilarNames(files);

            // group by similarly named, i.e. up to first open brace
            return files;
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
            Console.WriteLine($"Truncating '{fullPath}'");
            var name = new FileInfo(fullPath).Name;
            var orderedDelimiters = delimiter.ToDictionary(a => a, b => name.IndexOf(b)).OrderBy(c => c.Value).ToList();
            if(!orderedDelimiters.Any(x => x.Value > 0))
            {
                return name;
            }
            
            var firstDelimiter = orderedDelimiters.FirstOrDefault(x => x.Value > 0);
            return name.Substring(0, name.IndexOf(firstDelimiter.Key));
        }

        private static List<string> GetMostSuitable(
            List<string> duplicates,
            params string[] priorityTokens)
        {
            if (duplicates.Count == 1)
            {
                return new List<string>
                {
                    duplicates[0]
                };
            }

            foreach (var curToken in priorityTokens)
            {
                if (duplicates.Any(s => s.Contains(curToken, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return duplicates.Where(s => s.Contains(curToken, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }
            }

            Console.WriteLine($"None suitable for '{duplicates[0]}'.");
            return new List<string>();
        }
    }
}

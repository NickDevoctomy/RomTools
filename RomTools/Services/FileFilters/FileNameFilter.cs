using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return files;
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

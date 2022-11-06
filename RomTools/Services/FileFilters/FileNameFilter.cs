using RomTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Dictionary<string, object> options,
            Action<string, bool> log)
        {
            log($"Files before filtering ({files.Count}):", true);
            files.ForEach(x => log(x.FullPath, true));

            var groupedBySimilarNames = GroupBySimilarNames(files);

            var language = options["language"].ToString();
            var languageTokens = Program.Config.Languages[language].Split(',');
            var mostSuitable = groupedBySimilarNames
                .Select(x => GetMostSuitableByToken(x.Value, "()", languageTokens))
                .ToList();

            if ((bool)options["verified"])
            {
                mostSuitable = mostSuitable
                .Select(x => GetMostSuitableByToken(x, "[]", "!"))
                .ToList();
            }
            
            var filtered = mostSuitable.SelectMany(x => x).ToList();

            log("Most suitable files picked,", true);
            filtered.ForEach(x => log(x.FullPath, true));

            return filtered;
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
            return name.Substring(0, name.IndexOf(firstDelimiter.Key)).Trim();
        }

        private static List<FileEnvelope> GetMostSuitableByToken(
            List<FileEnvelope> duplicates,
            string braces,
            params string[] priorityTokens)
        {
            if(braces.Length != 2)
            {
                throw new ArgumentException("Braces must be 2 characters in length, for example \"()\" or \"[]\".");
            }

            var tokens = string.Join('|', priorityTokens);
            foreach (var curToken in priorityTokens)
            {
                var regex = $"[{braces[0]}].*({tokens}).*[{braces[1]}]";
                if (duplicates.Any(x => Regex.IsMatch(x.FullPath, regex, RegexOptions.IgnoreCase)))
                {
                    return duplicates.Where(x => Regex.IsMatch(x.FullPath, regex, RegexOptions.IgnoreCase)).ToList();
                }
            }

            //Console.WriteLine($"None suitable for '{duplicates[0].FullPath}'.");
            return new List<FileEnvelope>();
        }
    }
}

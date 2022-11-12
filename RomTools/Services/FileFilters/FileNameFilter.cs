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

        private readonly ITokenExtractorService _tokenExtractorService;

        public FileNameFilter(ITokenExtractorService tokenExtractorService)
        {
            _tokenExtractorService = tokenExtractorService;
        }

        public bool IsApplicable(PruneRomsOptions options)
        {
            return true;
        }

        public List<FileEnvelope> Filter(
            List<FileEnvelope> files,
            Dictionary<string, object> options,
            Action<string, bool> log)
        {
            log($"Files before filtering ({files.Count}):", true);
            files.ForEach(x => log(x.FullPath, true));

            var groupedBySimilarNames = GroupBySimilarNames(files);

            var tokenProfile = options["tokenProfile"].ToString();
            var includeTokens = Program.Config.TokenProfiles[tokenProfile].Include.Split(',');
            var excludedTokens = Program.Config.TokenProfiles[tokenProfile].Exclude.Split(',');
            var mostSuitable = groupedBySimilarNames
                .Select(x => GetMostSuitableByToken(x.Value, includeTokens, excludedTokens))
                .ToList();

            if ((bool)options["verified"])
            {
                mostSuitable = mostSuitable
                .Select(x => GetMostSuitableByToken(x, new[] { "!" }, null))
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
                curFile.Properties.Add("RoundBrancedTokens", _tokenExtractorService.ExtractTokens(curFile, new[]{ "(", ")" }));
                curFile.Properties.Add("SquareBrancedTokens", _tokenExtractorService.ExtractTokens(curFile, new[] { "[", "]" }));

                var truncatedFileName = TruncateFileNameUptoFirst(curFile.FullPath, '(', '[', '.');
                if (!grouped.ContainsKey(truncatedFileName))
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
            string[] priorityTokens,
            string[] excludeTokens)
        {
            duplicates.ForEach(x =>
            {
                var roundBracedTokens = (List<string>)x.Properties["RoundBrancedTokens"];
                var squareBracedTokens = (List<string>)x.Properties["SquareBrancedTokens"];
                var priorityTokenMatchCount = priorityTokens.Where(x => roundBracedTokens.Any(y => y.Contains(x, StringComparison.InvariantCulture))).Count();
                x.Properties.Add("PriorityTokenMatchCount", priorityTokenMatchCount);
                var excludedRoundBracedTokenMatchCount = excludeTokens.Where(x => roundBracedTokens.Any(y => y.Contains(x, StringComparison.InvariantCulture))).Count();
                var excludedSquareBracedTokenMatchCount = excludeTokens.Where(x => squareBracedTokens.Any(y => y.Contains(x, StringComparison.InvariantCulture))).Count();
                x.Properties.Add("ExcludedTokenMatchCount", excludedRoundBracedTokenMatchCount + excludedSquareBracedTokenMatchCount);
            });

            var orderedByMatchCount = duplicates
                .Where(x => (int)x.Properties["ExcludedTokenMatchCount"] == 0)
                .Where(x => (int)x.Properties["PriorityTokenMatchCount"] > 0)
                .OrderByDescending(x => (int)x.Properties["PriorityTokenMatchCount"])
                .ToList();

            if(orderedByMatchCount.Count() > 0)
            {
                return new List<FileEnvelope>
                {
                    orderedByMatchCount.First()
                };
            }

            //Console.WriteLine($"None suitable for '{duplicates[0].FullPath}'.");*/
            return new List<FileEnvelope>();
        }

        private static string RemoveExclusions(
            string value,
            string[] exclusions)
        {
            if(exclusions == null ||
                exclusions.Length == 0)
            {
                return value;
            }

            var excluded = value;
            foreach(var curExclusion in exclusions)
            {
                excluded = excluded.Replace($"({curExclusion})", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }

            return excluded.Trim();
        }
    }
}

using RomTools.Models;
using System.Text.RegularExpressions;

namespace RomTools.Services
{
    public class TokenExtractorService : ITokenExtractorService
    {
        public List<string> ExtractTokens(
            FileEnvelope file,
            params string[] braces)
        {
            var tokens = new List<string>();
            var fileInfo = new FileInfo(file.FullPath);
            //var matches = Regex.Matches(fileInfo.Name, $"[{braces[0]}][a-zA-Z0-9, -]+[{braces[1]}]");
            var matches = Regex.Matches(fileInfo.Name, $"[{braces[0]}][^{braces[1]}]+[{braces[1]}]");
            matches.ToList().ForEach(y =>
            {
                if (!tokens.Contains(y.Value))
                {
                    tokens.Add(y.Value);
                }
            });

            return tokens;
        }
    }
}

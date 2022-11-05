using System.Security.Cryptography;

namespace RomTools.Services.DuplicateFileFilters
{
    public class Md5DuplicateFileFilter : IFileFilter
    {
        public string Description => "Filter duplicate files by their md5 hash.";

        private readonly IMd5HasherService _md5HasherService;

        public Md5DuplicateFileFilter(IMd5HasherService md5HasherService)
        {
            _md5HasherService = md5HasherService;
        }

        public List<FileEnvelope> Filter(
            List<FileEnvelope> files,
            Action<string, bool> log)
        {
            _md5HasherService.HashAll(files, out var groupedByHash);

            log("All duplicates found by Md5 hash.", true);
            log(string.Empty, true);
            foreach (var curDuplicate in groupedByHash.Keys)
            {
                if(groupedByHash[curDuplicate].Count > 1)
                {
                    log($"Md5Hash :: {curDuplicate}", true);
                    foreach (var curFile in groupedByHash[curDuplicate])
                    {
                        log($"{curFile.FullPath}", true);
                    }

                    log(string.Empty, true);
                }
            }

            return groupedByHash
                .Select(x => x.Value.First())
                .ToList();
        }

        private void StoreFileHash(
            FileEnvelope file,
            Dictionary<string, List<FileEnvelope>> hashGroups)
        {
            var hash = GetMD5Hash(file.FullPath);
            if (!hashGroups.ContainsKey(hash))
            {
                hashGroups.Add(hash, new List<FileEnvelope>());
            }

            hashGroups[hash].Add(file);
            file.Properties.Add("Md5Hash", hash);
        }

        private string GetMD5Hash(string fullPath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fullPath);
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
    }
}

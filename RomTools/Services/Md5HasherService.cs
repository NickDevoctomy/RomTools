using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RomTools.Services
{
    public class Md5HasherService : IMd5HasherService
    {
        public void HashAll(
            List<FileEnvelope> files,
            out Dictionary<string, List<FileEnvelope>> groupedByHash)
        {
            var fileHashes = new Dictionary<string, List<FileEnvelope>>();
            files.ForEach(x => StoreFileHash(x, fileHashes));
            groupedByHash = fileHashes;
        }

        private void StoreFileHash(
            FileEnvelope file,
            Dictionary<string, List<FileEnvelope>> hashes)
        {
            var hash = GetMD5Hash(file.FullPath);
            if (!hashes.ContainsKey(hash))
            {
                hashes.Add(hash, new List<FileEnvelope>());
            }

            hashes[hash].Add(file);
            file.Properties.Add("Md5Hash", hash);
        }

        private string GetMD5Hash(string fullPath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fullPath))
                {
                    return Convert.ToBase64String(md5.ComputeHash(stream));
                }
            }
        }
    }
}

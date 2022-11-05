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
        public void HashAll(List<FileEnvelope> files)
        {
            files.ForEach(x => StoreFileHash(x));
        }

        private void StoreFileHash(FileEnvelope file)
        {
            var hash = GetMD5Hash(file.FullPath);
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

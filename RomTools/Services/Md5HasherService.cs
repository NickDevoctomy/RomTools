using ICSharpCode.SharpZipLib.Zip;
using RomTools.Models;
using System.Security.Cryptography;

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
            var isArchived = IsArchived(file.FullPath, out var supported);
            var archivedRomName = default(string);
            var archivedRomHash = isArchived && supported ?
                GetArchivedRomHash(file.FullPath, out archivedRomName) :
                null;
            var unarchivedRomHash = GetUnarchivedRomHash(file.FullPath);

            file.Properties.Add("Archived", isArchived.ToString());
            file.Properties.Add("ArchivedRomName", archivedRomName);
            file.Properties.Add("ArchivedRomMd5Hash", archivedRomHash);
            file.Properties.Add("RawMd5Hash", unarchivedRomHash);
        }

        private bool IsArchived(
            string fullPath,
            out bool supported)
        {
            supported = false;
            try
            {
                using ZipFile zipFile = new(fullPath);
                supported = zipFile.Count == 1 && zipFile[0].CanDecompress;
                return true;
            }
            catch (ZipException)
            {
                // Do nothing.
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }

        private string GetArchivedRomHash(
            string fullPath,
            out string name)
        {
            using (ZipFile zipFile = new(fullPath))
            {
                name = zipFile[0].Name;
                using var zipEntryStream = zipFile.GetInputStream(0);
                using var md5 = MD5.Create();
                return Convert.ToBase64String(md5.ComputeHash(zipEntryStream));
            }
        }

        private string GetUnarchivedRomHash(string fullPath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fullPath);
            return Convert.ToBase64String(md5.ComputeHash(stream));
        }
    }
}

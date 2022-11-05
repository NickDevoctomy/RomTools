namespace RomTools.Services
{
    public class FileEnvelope
    {
        public string FullPath { get; set; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

        public FileEnvelope(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}

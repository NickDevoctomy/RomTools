namespace RomTools.Models
{
    public class FileEnvelope
    {
        public string FullPath { get; set; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public FileEnvelope(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}

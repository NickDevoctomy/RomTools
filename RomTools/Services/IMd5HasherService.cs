namespace RomTools.Services
{
    public interface IMd5HasherService
    {
        public void HashAll(
            List<FileEnvelope> files,
            out Dictionary<string, List<FileEnvelope>> groupedByHash);
    }
}

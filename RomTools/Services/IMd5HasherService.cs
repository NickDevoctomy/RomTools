namespace RomTools.Services
{
    public interface IMd5HasherService
    {
        public void HashAll(List<FileEnvelope> files);
    }
}

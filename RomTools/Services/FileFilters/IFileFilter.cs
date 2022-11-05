namespace RomTools.Services.FileFilters;

public interface IFileFilter
{
    public int Priority { get; }
    public string Description { get; }

    public List<FileEnvelope> Filter(
        List<FileEnvelope> files,
        Action<string, bool> log);
}

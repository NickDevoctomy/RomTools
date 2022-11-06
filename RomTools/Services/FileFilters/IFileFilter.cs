namespace RomTools.Services.FileFilters;

public interface IFileFilter
{
    public int Priority { get; }
    public string Description { get; }

    public List<FileEnvelope> Filter(
        List<FileEnvelope> files,
        Dictionary<string, object> options,
        Action<string, bool> log);
}

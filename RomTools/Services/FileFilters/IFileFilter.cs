using RomTools.Models;

namespace RomTools.Services.FileFilters;

public interface IFileFilter
{
    public int Priority { get; }
    public string Description { get; }

    public bool IsApplicable(PruneRomsOptions options);

    public List<FileEnvelope> Filter(
        List<FileEnvelope> files,
        Dictionary<string, object> options,
        Action<string, bool> log);
}

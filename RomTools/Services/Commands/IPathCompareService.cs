using RomTools.Models;

namespace RomTools.Services.Commands;

public interface IPathCompareService
{
    public Task<int> CompareAsync(
        PathCompareOptions options,
        CancellationToken cancellationToken);
}

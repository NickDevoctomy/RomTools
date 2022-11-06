using RomTools.Models;

namespace RomTools.Services;

public interface IPruneRomsService
{
    public Task<int> ProcessAsync(
        PruneRomsOptions options,
        CancellationToken cancellationToken);
}

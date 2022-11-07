using RomTools.Models;

namespace RomTools.Services.Commands;

public interface IPruneRomsService
{
    public Task<int> ProcessAsync(
        PruneRomsOptions options,
        CancellationToken cancellationToken);
}

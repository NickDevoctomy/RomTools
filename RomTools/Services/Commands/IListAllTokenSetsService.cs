using RomTools.Models;

namespace RomTools.Services.Commands;

public interface IListAllTokenSetsService
{
    public Task<int> ListAsync(
        ListAllTokensOptions options,
        CancellationToken cancellationToken);
}

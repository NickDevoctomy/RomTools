using RomTools.Models;

namespace RomTools.Services.Commands;

public interface IListAllTokensService
{
    public Task<int> ListAsync(
        ListAllTokensOptions options,
        CancellationToken cancellationToken);
}

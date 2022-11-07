using RomTools.Models;

namespace RomTools.Services.Commands;

public interface IListAllLanguagesService
{
    public Task<int> ListAsync(
        ListAllLanguagesOptions options,
        CancellationToken cancellationToken);
}

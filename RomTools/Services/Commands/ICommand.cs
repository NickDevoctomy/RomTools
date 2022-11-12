using RomTools.Models;

namespace RomTools.Services.Commands
{
    public interface ICommand
    {
        public bool IsApplicable(PreOptions preOptions);

        public Task<int> RunAsync(
            string arguments,
            CancellationToken cancellationToken);
    }
}

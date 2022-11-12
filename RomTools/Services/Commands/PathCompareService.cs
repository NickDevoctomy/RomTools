using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;

namespace RomTools.Services.Commands
{
    public class PathCompareService : IPathCompareService, ICommand
    {
        private readonly ICommandLineParserService _commandLineParserService;

        public PathCompareService(ICommandLineParserService commandLineParserService)
        {
            _commandLineParserService = commandLineParserService;
        }

        public Task<int> CompareAsync(
            PathCompareOptions options,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool IsApplicable(PreOptions preOptions)
        {
            return preOptions.Command == Enums.Command.PathCompare;
        }

        public async Task<int> RunAsync(
            string arguments,
            CancellationToken cancellationToken)
        {
            await Task.Yield();

            return (int)ReturnCodes.Unknown;
        }
    }
}

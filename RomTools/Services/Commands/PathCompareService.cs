using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;

namespace RomTools.Services.Commands
{
    public class PathCompareService : IPathCompareService, ICommand
    {
        private readonly ICommandLineParserService _commandLineParserService;
        private readonly IMd5HasherService _md5HasherService;

        public PathCompareService(
            ICommandLineParserService commandLineParserService,
            IMd5HasherService md5HasherService)
        {
            _commandLineParserService = commandLineParserService;
            _md5HasherService = md5HasherService;
        }

        public async Task<int> CompareAsync(
            PathCompareOptions options,
            CancellationToken cancellationToken)
        {
            await Task.Yield();

            LogMessage($"Getting all files from path A '{options.PathA}'.", false, ConsoleColor.White, options);
            var filesA = GetAllFilesFromPath(options.PathA);

            LogMessage($"Getting all files from path B '{options.PathB}'.", false, ConsoleColor.White, options);
            var filesB = GetAllFilesFromPath(options.PathB);

            LogMessage($"Hashing all files from A.", false, ConsoleColor.White, options);
            _md5HasherService.HashAll(filesA, false);
            LogMessage($"All files from A hashed.", false, ConsoleColor.White, options);

            LogMessage($"Hashing all files from B.", false, ConsoleColor.White, options);
            _md5HasherService.HashAll(filesB, false);
            LogMessage($"All files from B hashed.", false, ConsoleColor.White, options);

            LogMessage($"Getting files in A but not in B.", false, ConsoleColor.White, options);
            var filesInANotB = filesA
                .Where(x => !filesB.Any(y => y.Properties["Md5Hash"].Equals(x.Properties["Md5Hash"])))
                .ToList();

            LogMessage($"Getting files in B but not in A.", false, ConsoleColor.White, options);
            var filesInBNotA = filesB
                .Where(x => !filesA.Any(y => y.Properties["Md5Hash"].Equals(x.Properties["Md5Hash"])))
                .ToList();

            if(filesInANotB.Count > 0)
            {
                LogMessage($"The following {filesInANotB.Count} files are present in A but not in B,", false, ConsoleColor.White, options);
                filesInANotB.ForEach(x => LogMessage(x.FullPath, false, ConsoleColor.Blue, options));
            }

            if (filesInBNotA.Count > 0)
            {
                LogMessage($"The following {filesInBNotA.Count} files are present in B but not in A,", false, ConsoleColor.White, options);
                filesInBNotA.ForEach(x => LogMessage(x.FullPath, false, ConsoleColor.Blue, options));
            }

            return (int)ReturnCodes.Success;
        }

        public bool IsApplicable(PreOptions preOptions)
        {
            return preOptions.Command == Enums.Command.PathCompare;
        }

        public async Task<int> RunAsync(
            string arguments,
            CancellationToken cancellationToken)
        {
            if (_commandLineParserService.TryParseArgumentsAsOptions(
                typeof(PathCompareOptions),
                arguments,
                out var pathCompareOptions))
            {
                return await CompareAsync(
                    (PathCompareOptions)pathCompareOptions.Options,
                    cancellationToken);
            }
            else
            {
                Console.WriteLine($"{pathCompareOptions.Exception.Message}");
            }

            return (int)ReturnCodes.Unknown;
        }

        private static List<FileEnvelope> GetAllFilesFromPath(string path)
        {
            var allFiles = Directory.GetFiles(path, "*.*").ToList();
            return allFiles.Select(x => new FileEnvelope(x)).ToList();
        }

        private static void LogMessage(
            string message,
            bool isVerbose,
            ConsoleColor color,
            PathCompareOptions options)
        {
            if (!isVerbose || isVerbose && options.Verbose)
            {
                var prevColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = prevColor;
            }
        }
    }
}

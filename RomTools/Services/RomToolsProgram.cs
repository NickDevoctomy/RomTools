using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Enums;
using System.Text;

namespace RomTools.Services
{
    public class RomToolsProgram : IProgram
    {
        private readonly ICommandLineArgumentService _commandLineArgumentService;
        private readonly ICommandLineParserService _commandLineParserService;
        private readonly IHelpMessageFormatter _helpMessageFormatter;
        private readonly IPruneRomsService _pruneRomsService;
        private readonly ICreateHashedCollectionService _createHashedCollectionService;

        public RomToolsProgram(
            ICommandLineArgumentService commandLineArgumentService,
            ICommandLineParserService commandLineParserService,
            IHelpMessageFormatter helpMessageFormatter,
            IPruneRomsService pruneRomsService,
            ICreateHashedCollectionService createHashedCollectionService)
        {
            _commandLineArgumentService = commandLineArgumentService;
            _commandLineParserService = commandLineParserService;
            _helpMessageFormatter = helpMessageFormatter;
            _pruneRomsService = pruneRomsService;
            _createHashedCollectionService = createHashedCollectionService;
        }

        public async Task<int> Run(CancellationToken cancellationToken)
        {
            var returnCode = 0;
            var arguments = _commandLineArgumentService.GetArguments(Environment.CommandLine);
            if (_commandLineParserService.TryParseArgumentsAsOptions(typeof(PreOptions), arguments, out var preOptions))
            {
                switch (preOptions.OptionsAs<PreOptions>().Command)
                {
                    case Command.PruneRoms:
                        {
                            if (_commandLineParserService.TryParseArgumentsAsOptions(
                                typeof(PruneRomsOptions),
                                arguments,
                                out var pruneRomsOptions))
                            {
                                returnCode = await _pruneRomsService.ProcessAsync(
                                    (PruneRomsOptions)pruneRomsOptions.Options,
                                    cancellationToken);
                            }
                            else
                            {
                                Console.WriteLine($"{pruneRomsOptions.Exception.Message}");
                            }

                            break;
                        }

                    case Command.CreateHashedCollection:
                        {
                            if (_commandLineParserService.TryParseArgumentsAsOptions(
                                typeof(CreateHashedCollectionOptions),
                                arguments,
                                out var createHashedCollectionOptions))
                            {
                                returnCode = await _createHashedCollectionService.CreateAsync(
                                    (CreateHashedCollectionOptions)createHashedCollectionOptions.Options,
                                    cancellationToken);
                            }
                            else
                            {
                                Console.WriteLine($"{createHashedCollectionOptions.Exception.Message}");
                            }

                            break;
                        }

                    default:
                        {
                            Console.WriteLine($"Command {preOptions.OptionsAs<PreOptions>().Command} not yet implemented.");
                            break;
                        }
                }
            }
            else
            {
                string strExeFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
                var helpMessage = _helpMessageFormatter.Format<PreOptions>();
                var message = new StringBuilder();
                if(preOptions.InvalidOptions.Count == 0)
                {
                    message.AppendLine(preOptions.Exception.Message);
                    returnCode = (int)ReturnCodes.NoArguments;
                }
                else
                {
                    if (preOptions.InvalidOptions.ContainsKey("Command"))
                    {
                        var expected = Enum.GetNames<Command>().Where(x => x != "None").Select(y => y.ToLower()).ToList();
                        message.AppendLine($"Unknown command '{preOptions.InvalidOptions["Command"]}', expected one of ({string.Join(',', expected)}).");
                        returnCode = (int)ReturnCodes.InvalidArguments;
                    }
                    else
                    {
                        message.AppendLine($"Invalid command line '{Environment.CommandLine}'.");
                        returnCode = (int)ReturnCodes.IncorrectArgumentSyntax;
                    }
                }

                message.AppendLine();
                message.AppendLine($"Usage: {new FileInfo(strExeFilePath).Name} [command] [command_options]");
                message.AppendLine();
                message.Append(helpMessage);
                Console.WriteLine(message);
            }

            return returnCode;
        }
    }
}

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

        public RomToolsProgram(
            ICommandLineArgumentService commandLineArgumentService,
            ICommandLineParserService commandLineParserService,
            IHelpMessageFormatter helpMessageFormatter,
            IPruneRomsService pruneRomsService)
        {
            _commandLineArgumentService = commandLineArgumentService;
            _commandLineParserService = commandLineParserService;
            _helpMessageFormatter = helpMessageFormatter;
            _pruneRomsService = pruneRomsService;
        }

        public async Task<int> Run()
        {
            await Task.Yield(); // !!! Remove this

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
                                return await _pruneRomsService.Process((PruneRomsOptions)pruneRomsOptions.Options);
                            }
                            else
                            {
                                Console.WriteLine($"{pruneRomsOptions.Exception.Message}");
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
                }
                else
                {
                    if (preOptions.InvalidOptions.ContainsKey("Command"))
                    {
                        var expected = Enum.GetNames<Command>().Where(x => x != "None").Select(y => y.ToLower()).ToList();
                        message.AppendLine($"Unknown command '{preOptions.InvalidOptions["Command"]}', expected one of ({string.Join(',', expected)}).");
                    }
                    else
                    {
                        message.AppendLine($"Invalid command line '{Environment.CommandLine}'.");
                    }
                }

                message.AppendLine();
                message.AppendLine($"Usage: {new FileInfo(strExeFilePath).Name} [command] [command_options]");
                message.AppendLine();
                message.Append(helpMessage);
                Console.WriteLine(message);
            }

            return -1;
        }
    }
}

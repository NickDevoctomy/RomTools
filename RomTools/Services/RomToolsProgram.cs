﻿using RomTools.Models;
using RomTools.Services.CommandLineParser;
using RomTools.Services.Commands;
using RomTools.Services.Enums;
using System.Text;

namespace RomTools.Services
{
    public class RomToolsProgram : IProgram
    {
        private readonly IArgumentsFlattenerService _argumentsFlattenerService;
        private readonly ICommandLineParserService _commandLineParserService;
        private readonly IHelpMessageFormatter _helpMessageFormatter;
        private readonly IEnumerable<ICommand> _commands;

        public RomToolsProgram(
            IArgumentsFlattenerService argumentFlattenerService,
            ICommandLineParserService commandLineParserService,
            IHelpMessageFormatter helpMessageFormatter,
            IEnumerable<ICommand> commands)
        {
            _argumentsFlattenerService = argumentFlattenerService;
            _commandLineParserService = commandLineParserService;
            _helpMessageFormatter = helpMessageFormatter;
            _commands = commands;
        }

        public async Task<int> Run(string[] args, CancellationToken cancellationToken)
        {
            var returnCode = 0;
            var arguments = _argumentsFlattenerService.Flatten(args);
            if (_commandLineParserService.TryParseArgumentsAsOptions(
                typeof(PreOptions),
                arguments,
                out var preOptions))
            {
                var command = _commands.SingleOrDefault(x => x.IsApplicable(preOptions.OptionsAs<PreOptions>()));
                if(command != null)
                {
                    returnCode = await command.RunAsync(
                        arguments,
                        cancellationToken);
                }
                else
                {
                    Console.WriteLine($"Command {preOptions.OptionsAs<PreOptions>().Command} not yet implemented.");
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

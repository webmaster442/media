using FFCmd.Dto;
using FFCmd.Infrastructure;
using FFCmd.Infrastructure.BaseCommands;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal sealed class BachCompile : BaseBachCommand<BachCompile.Settings>
{
    private readonly CommandApp _bachApp;
    private readonly Dictionary<string, string> _commandNamesAndExtensions;
    private readonly IDryRunResultAcceptor _dryRunResultAcceptor;

    public class Settings : BaseBachSettings
    {
    }

    public BachCompile()
    {
        var services = new ServiceCollection();

        _dryRunResultAcceptor = new DryRunResultAcceptor();

        services.AddSingleton(_dryRunResultAcceptor);
        var registar = new TypeRegistrar(services);

        _bachApp = new CommandApp(registar);
        _bachApp.Configure(c =>
        {
            c.AddCommand<ConvertToAlac>("alac");
            c.AddCommand<ConvertToFlac>("flac");
            c.AddCommand<ConvertToM4a>("m4a");
            c.AddCommand<ConvertToMp3>("mp3");
            c.AddCommand<ConvertToCdWav>("cdwav");
            c.AddCommand<ConvertContactSheet>("contactsheet");
        });

        _commandNamesAndExtensions = new()
        {
            { "alac", "m4a" },
            { "flac", "flac" },
            { "m4a", "m4a" },
            { "mp3", "mp3" },
            { "cdwav", "wav" },
            { "contactsheet", "jpg" }
        };
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            BachProject project = await LoadProject(settings.ProjectName);

            string[] cmdLine =
            [
                project.ConversionCommand, .. project.Args, "input", "output",
            ];

            foreach (var inputFile in project.Files)
            {

                var fileName = Path.GetFileNameWithoutExtension(inputFile);
                var outputFile = Path.Combine(project.OutputDirectory, Path.ChangeExtension(fileName, _commandNamesAndExtensions[project.ConversionCommand]));
                cmdLine[^2] = inputFile;
                cmdLine[^1] = outputFile;
                _bachApp.Run(cmdLine);

                var generated = _dryRunResultAcceptor.Result;
            }

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}

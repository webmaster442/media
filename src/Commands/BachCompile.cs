// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

namespace Media.Commands;

internal sealed class BachCompile : BaseBachCommand<BachCompile.Settings>
{
    private readonly CommandApp _bachApp;
    private readonly Dictionary<string, string> _commandNamesAndExtensions;
    private readonly IDryRunResultAcceptor _dryRunResultAcceptor;

    public class Settings : BaseBachSettings
    {
        public enum ShellType
        {
            Powershell,
            Cmd,
        }

        [Description("Output shell type")]
        [CommandOption("-s|--shell")]
        public ShellType OutputType { get; set; } = ShellType.Powershell;
    }

    public BachCompile()
    {
        _dryRunResultAcceptor = new DryRunResultAcceptor
        {
            Enabled = true
        };

        _bachApp = new CommandApp(ProgramFactory.CreateTypeRegistar(_dryRunResultAcceptor));
        _bachApp.Configure(c =>
        {
            c.AddCommand<ConvertToAlac>("alac");
            c.AddCommand<ConvertToFlac>("flac");
            c.AddCommand<ConvertToM4a>("m4a");
            c.AddCommand<ConvertToMp3>("mp3");
            c.AddCommand<ConvertToCdWav>("cdwav");
            c.AddCommand<ConvertToDVDWav>("dvdwav");
            c.AddCommand<ConvertToAc3>("ac3");
            c.AddCommand<ConvertContactSheet>("contactsheet");
            c.AddCommand<ConvertNtscDvd>("dvd-ntsc");
            c.AddCommand<ConvertPalDvd>("dvd-pal");
        });

        _commandNamesAndExtensions = new()
        {
            { "alac", "m4a" },
            { "flac", "flac" },
            { "m4a", "m4a" },
            { "mp3", "mp3" },
            { "cdwav", "wav" },
            { "ac3", "ac3" },
            { "dvd-ntsc", "mpg" },
            { "dvd-pal", "mpg" },
            { "contactsheet", "jpg" }
        };
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        BachProject project = await LoadProject(settings.ProjectName);

        var directory = Path.GetDirectoryName(settings.ProjectName)
            ?? throw new InvalidOperationException("Couldn't get directory name");

        var scriptPath = Path.ChangeExtension(Path.GetFileNameWithoutExtension(settings.ProjectName), ".ps1");

        string[] cmdLine =
        [
            project.ConversionCommand, .. project.Args, "input", "output",
            ];

        if (!FFMpeg.TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found.");
        }

        IShellBuilder builder = settings.OutputType == Settings.ShellType.Powershell
            ? new PowershellBuilder()
            : new CmdBuilder();

        builder.WithUtf8Enabled();
        builder.WithWindowTitle(Path.GetFileNameWithoutExtension(settings.ProjectName));
        builder.WithClear();

        foreach (var inputFile in project.Files)
        {

            var fileName = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(project.OutputDirectory, Path.ChangeExtension(fileName, _commandNamesAndExtensions[project.ConversionCommand]));
            cmdLine[^2] = inputFile;
            cmdLine[^1] = outputFile;
            _bachApp.Run(cmdLine);

            var generated = _dryRunResultAcceptor.Result;

            builder.WithCommandIfFileNotExists(outputFile, $"\"{ffmpegPath}\" {generated}");
        }

        using var scriptWriter = File.CreateText(Path.Combine(directory, scriptPath));

        await scriptWriter.WriteAsync(builder.Build());
    }
}

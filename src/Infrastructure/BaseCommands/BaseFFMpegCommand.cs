using System.Diagnostics;

using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Infrastructure.BaseCommands;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
    : Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    private readonly ICommandConfig? _commandSettings;

    protected BaseFFMpegCommand(ICommandConfig? commandSettings = null)
    {
        _commandSettings = commandSettings;
        GeneratedCommandLine = string.Empty;
    }

    public string GeneratedCommandLine { get; private set; }

    public override int Execute(CommandContext context, TBaseFFMpegSettings settings)
    {
        try
        {
            FFMpegCommandBuilder builder = new();
            BuildCommandLine(builder, settings);

            var cmdLine = builder.BuildCommandLine();
           
            if (_commandSettings != null &&
                _commandSettings.Mode == ExectuionMode.DryRun)
            {
                DryRun(cmdLine);
            }
            else
            {
                Execute(cmdLine);
            }

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }

    private void DryRun(string commandLine)
    {
        GeneratedCommandLine = commandLine;
    }

    private void Execute(string commandLine)
    {
        DryRun(commandLine);
        Terminal.InfoText("Generated arguments:");
        Terminal.InfoText(GeneratedCommandLine);
        FFMpeg.StartFFMpeg(commandLine);
    }

    protected abstract void BuildCommandLine(FFMpegCommandBuilder builder, TBaseFFMpegSettings settings);
}
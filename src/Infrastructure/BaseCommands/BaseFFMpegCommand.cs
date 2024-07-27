using FFCmd.Interop;

using Spectre.Console.Cli;

namespace FFCmd.Infrastructure.BaseCommands;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
    : Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    public override int Execute(CommandContext context, TBaseFFMpegSettings settings)
    {
        try
        {
            FFMpegCommandBuilder builder = new();
            BuildCommandLine(builder, settings);

            var cmdLine = builder.BuildCommandLine();
            Terminal.InfoText("Generated arguments:");
            Terminal.InfoText(cmdLine);

            FFMpeg.StartFFMpeg(cmdLine);
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }

    protected abstract void BuildCommandLine(FFMpegCommandBuilder builder, TBaseFFMpegSettings settings);
}
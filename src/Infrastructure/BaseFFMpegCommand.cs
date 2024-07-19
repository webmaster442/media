using FFCmd.Interop;

using Spectre.Console;
using Spectre.Console.Cli;

namespace FFCmd.Infrastructure;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
    : Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    public override int Execute(CommandContext context, TBaseFFMpegSettings settings)
    {
        try
        {
            FFMpegCommandBuilder builder = new();
            BuildCommandLine(builder, settings);
            FFMpeg.StartFFMpeg(builder);
            return 0;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return -1;
        }
    }

    protected abstract void BuildCommandLine(FFMpegCommandBuilder builder, TBaseFFMpegSettings settings);
}
using FFCmd.FFMpegInterop;

using Spectre.Console.Cli;

namespace FFCmd.Infrastructure;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
    : Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    public override int Execute(CommandContext context, TBaseFFMpegSettings settings)
    {
        FFMpegCommandBuilder builder = new();
        BuildCommandLine(builder, settings);
        FFMpeg.StartFFMpeg(builder);
        return 0;
    }

    protected abstract void BuildCommandLine(FFMpegCommandBuilder builder, TBaseFFMpegSettings settings);
}
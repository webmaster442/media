// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Media.Interop;

namespace Media.Infrastructure.BaseCommands;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
: Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    private readonly ConfigAccessor _configAccessor;
    private readonly FFMpeg _ffMpeg;

    protected BaseFFMpegCommand(ConfigAccessor configAccessor)
    {
        _configAccessor = configAccessor;
        _ffMpeg = new FFMpeg(_configAccessor);
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] TBaseFFMpegSettings settings)
    {
        FFMpegCommandBuilder builder = new();
        BuildCommandLine(builder, settings);

        var cmdLine = builder.Build();

        Terminal.InfoText("Generated arguments:");
        Terminal.InfoText(cmdLine);
        _ffMpeg.Start(cmdLine);

        return ExitCodes.Success;
    }

    protected abstract void BuildCommandLine(FFMpegCommandBuilder builder, TBaseFFMpegSettings settings);
}
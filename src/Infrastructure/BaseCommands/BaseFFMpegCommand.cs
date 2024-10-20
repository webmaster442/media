// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Media.Interfaces;
using Media.Interop;

namespace Media.Infrastructure.BaseCommands;

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
: Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    private readonly ConfigAccessor _configAccessor;
    private readonly IDryRunResultAcceptor _dryRunResultAcceptor;
    private readonly FFMpeg _ffMpeg;

    protected BaseFFMpegCommand(ConfigAccessor configAccessor, IDryRunResultAcceptor dryRunResultAcceptor)
    {
        _configAccessor = configAccessor;
        _dryRunResultAcceptor = dryRunResultAcceptor;
        _ffMpeg = new FFMpeg(_configAccessor);
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] TBaseFFMpegSettings settings)
    {
        try
        {
            FFMpegCommandBuilder builder = new();
            BuildCommandLine(builder, settings);

            var cmdLine = builder.Build();

            if (_dryRunResultAcceptor.Enabled)
            {
                _dryRunResultAcceptor.Result = cmdLine;
            }
            else
            {
                Terminal.InfoText("Generated arguments:");
                Terminal.InfoText(cmdLine);
                _ffMpeg.Start(cmdLine);
            }

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
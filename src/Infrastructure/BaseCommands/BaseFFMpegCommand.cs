using System.Diagnostics.CodeAnalysis;

using Media.Interop;

namespace Media.Infrastructure.BaseCommands;

internal interface IDryRunResultAcceptor
{
    string Result { get; set; }
}

internal sealed class DryRunResultAcceptor : IDryRunResultAcceptor
{
    public string Result { get; set; } = string.Empty;
}

internal abstract class BaseFFMpegCommand<TBaseFFMpegSettings>
: Command<TBaseFFMpegSettings> where TBaseFFMpegSettings : BaseFFMpegSettings
{
    private readonly IDryRunResultAcceptor? _dryRunResultAcceptor;

    protected BaseFFMpegCommand(IDryRunResultAcceptor? dryRunResultAcceptor)
    {
        _dryRunResultAcceptor = dryRunResultAcceptor;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] TBaseFFMpegSettings settings)
    {
        try
        {
            FFMpegCommandBuilder builder = new();
            BuildCommandLine(builder, settings);

            var cmdLine = builder.Build();

            if (_dryRunResultAcceptor != null)
            {
                _dryRunResultAcceptor.Result = cmdLine;
            }
            else
            {
                Terminal.InfoText("Generated arguments:");
                Terminal.InfoText(cmdLine);
                FFMpeg.Start(cmdLine);
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
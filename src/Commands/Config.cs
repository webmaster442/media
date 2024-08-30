using System.Diagnostics;

using Media.Infrastructure;

namespace Media.Commands;

internal sealed class Config : AsyncCommand
{
    private readonly ConfigAccessor _configAccessor;

    public Config(ConfigAccessor configAccessor)
    {
        _configAccessor = configAccessor;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            await _configAccessor.ForceSave();

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _configAccessor.ConfigPath,
                    UseShellExecute = true,
                },
            };
            process.Start();

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Error;
        }
    }
}

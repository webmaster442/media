namespace Media.Commands;

internal sealed class UpdateAll : AsyncCommand
{
    private readonly AsyncCommand[] _updateCommands;

    public UpdateAll()
    {
        _updateCommands = [new UpdateFFMpeg(), new UpdateMpv(), new UpdateYtdlp()];
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        foreach (var command in _updateCommands)
        {
            var result = await command.ExecuteAsync(context);
            if (result != ExitCodes.Success)
            {
                return result;
            }
        }
        return ExitCodes.Success;
    }
}
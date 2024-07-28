namespace FFCmd;

internal interface ICommandConfig
{
    public ExectuionMode Mode { get; set; }
}

internal class CommandConfig : ICommandConfig
{
    public ExectuionMode Mode { get; set; } = ExectuionMode.DryRun;
}

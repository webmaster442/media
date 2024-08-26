using Media.Infrastructure;
using Media.Infrastructure.Dlna;
using Media.Infrastructure.Validation;

namespace Media.Commands;

internal class ServeCommand : AsyncCommand<ServeCommand.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [Required]
        [Description("The folder to serve via DLNA. If not specified, current folder is used")]
        [CommandArgument(0, "[<folder]")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var server = new DLNAServer(8080, settings.Folder);
        await server.RunAsync(CancellationToken.None);
        return ExitCodes.Success;
    }
}

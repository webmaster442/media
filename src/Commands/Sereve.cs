using Media.Infrastructure;
using Media.Infrastructure.Dlna;
using Media.Infrastructure.Validation;

using NMaier.SimpleDlna.Server.Http;

using Spectre.Console;

namespace Media.Commands;

internal class Sereve : Command<Sereve.Settings>
{
    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to serve.")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        try
        {
            using var server = new HttpServer(8085);
            server.RegisterMediaServer(new MediaServer());
            AnsiConsole.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
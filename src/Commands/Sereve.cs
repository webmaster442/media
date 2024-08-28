using System.Diagnostics.CodeAnalysis;

using Media.Infrastructure;
using Media.Infrastructure.Validation;

using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Types;

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

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        try
        {
            using var server = new HttpServer(8085);
            DirectoryInfo directory = new DirectoryInfo(settings.Folder);

            using var fileServer = new FileServer(DlnaMediaTypes.All, new Identifiers(new TitleComparer(), false), directory);
            server.RegisterMediaServer(fileServer);
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
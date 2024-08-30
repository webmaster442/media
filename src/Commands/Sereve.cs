using System.Diagnostics.CodeAnalysis;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;

using Media.Infrastructure;
using Media.Infrastructure.Validation;

using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Types;

namespace Media.Commands;

internal sealed class Sereve : Command<Sereve.Settings>
{
    private readonly int _dlnaServerPort;

    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to serve.")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    public Sereve(ConfigAccessor configAccessor)
    {
        _dlnaServerPort = configAccessor.GetDlnaServerPort() ?? 8085;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        try
        {
            using var server = new HttpServer(_dlnaServerPort);
            server.Logger.IsEnabledFor(log4net.Core.Level.Error);

            ILoggerRepository repository = LogManager.GetRepository();
            BasicConfigurator.Configure(repository, new ConsoleAppender());

            DirectoryInfo directory = new(settings.Folder);

            using var fileServer = new FileServer(DlnaMediaTypes.All,
                                                  new Identifiers(new TitleComparer(), false),
                                                  directory);
            fileServer.Load();
            server.RegisterMediaServer(fileServer);

            Terminal.GreenText("Media server running...");
            Terminal.InfoText("Press any key to stop the server...");
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
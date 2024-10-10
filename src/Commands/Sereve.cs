// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Media.Infrastructure;
using Media.Infrastructure.Validation;

using Microsoft.Extensions.Logging;

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
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.AddFilter(loglevel => loglevel >= LogLevel.Information);
            });


            using var server = new HttpServer(_dlnaServerPort, loggerFactory);

            DirectoryInfo directory = new(settings.Folder);

            using var fileServer = new FileServer(DlnaMediaTypes.All,
                                                  new Identifiers(new TitleComparer(), loggerFactory, false),
                                                  loggerFactory, directory);
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
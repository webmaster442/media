using FFCmd.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace FFCmd.Commands;

internal class BachCompile
{
    private readonly CommandApp _bachApp;

    public BachCompile()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICommandConfig>(new CommandConfig
        {
            Mode = ExectuionMode.DryRun
        });
        var registar = new TypeRegistrar(services);

        _bachApp = new CommandApp(registar);
        _bachApp.Configure(c =>
        {
            c.AddCommand<ConvertToAlac>("alac");
            c.AddCommand<ConvertToFlac>("flac");
            c.AddCommand<ConvertToM4a>("m4a");
            c.AddCommand<ConvertToMp3>("mp3");
            c.AddCommand<ConvertToCdWav>("cdwav");
            c.AddCommand<ConvertContactSheet>("contactsheet");
        });
    }

}

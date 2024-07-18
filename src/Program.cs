using FFCmd.Commands;

using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<ConvertFlac>("convert-to-flac");
});

app.Run(args);
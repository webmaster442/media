using FFCmd.Commands;

using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<ConvertFlac>("convert-to-flac");
    config.AddCommand<ConvertToMp3>("convert-to-mp3");
});

app.Run(args);
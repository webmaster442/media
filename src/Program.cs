using FFCmd.Commands;

using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddBranch("convert", convert =>
    {
        convert.AddCommand<ConvertToAlac>("alac");
        convert.AddCommand<ConvertToFlac>("flac");
        convert.AddCommand<ConvertToM4a>("m4a");
        convert.AddCommand<ConvertToMp3>("mp3");
    });
    config.AddBranch("extract", extract =>
    {
        extract.AddCommand<ExtractAudioStereoM4a>("stereo-m4a");
        extract.AddCommand<ExtractAudioCopy>("copy");
    });
});

app.Run(args);

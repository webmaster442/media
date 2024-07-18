using FFCmd.Commands;

using JKToolKit.Spectre.AutoCompletion.Completion;
using JKToolKit.Spectre.AutoCompletion.Integrations;

using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddAutoCompletion(config => config.AddPowershell());
    config.AddBranch("convert", convert =>
    {
        convert.SetDescription("Convert audio/video files");
        convert.AddCommand<ConvertToAlac>("alac");
        convert.AddCommand<ConvertToFlac>("flac");
        convert.AddCommand<ConvertToM4a>("m4a");
        convert.AddCommand<ConvertToMp3>("mp3");
    });
    config.AddBranch("extract", extract =>
    {
        extract.SetDescription("Extract audio/video stream from files");
        extract.AddCommand<ExtractAudioStereoM4a>("stereo-m4a");
        extract.AddCommand<ExtractAudioCopy>("copy");
    });
    config.AddBranch("update", update =>
    {
        update.SetDescription("Update related commands");
        update.AddCommand<UpdateFFMpeg>("ffmpeg");
    });
    config.AddBranch("mux", mux =>
    {
        mux.SetDescription("mux releated commands");
        mux.AddCommand<MuxAddAudio>("add-audio");
    });

});

app.Run(args);

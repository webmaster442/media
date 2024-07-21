using FFCmd.Commands;
using FFCmd.Infrastructure;

using JKToolKit.Spectre.AutoCompletion.Completion;
using JKToolKit.Spectre.AutoCompletion.Integrations;

using Spectre.Console.Cli;

var mainApp = new CommandApp();

Terminal.EnableUTF8Output();

mainApp.Configure(config =>
{
    config.SetApplicationName("FFCmd");
    config.SetApplicationCulture(System.Globalization.CultureInfo.InvariantCulture);
    config.AddAutoCompletion(config => config.AddPowershell());

    config.AddCommand<Info>("info").WithDescription("Get information about a media file");
    config.AddCommand<Cut>("cut").WithDescription("Cut a file without reencoding");
    config.AddBranch("convert", convert =>
    {
        convert.SetDescription("Convert audio/video files");
        convert.AddCommand<ConvertToAlac>("alac");
        convert.AddCommand<ConvertToFlac>("flac");
        convert.AddCommand<ConvertToM4a>("m4a");
        convert.AddCommand<ConvertToMp3>("mp3");
        convert.AddCommand<ConvertToCdWav>("cdwav");
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
        update.AddCommand<UpdateMpv>("mpv");
        update.AddCommand<UpdateAll>("all");
    });
    config.AddBranch("mux", mux =>
    {
        mux.SetDescription("mux releated commands");
        mux.AddCommand<MuxAddAudio>("add-audio");
        mux.AddCommand<MuxAddSubtitle>("add-subtitle");
    });
});

mainApp.Run(args);

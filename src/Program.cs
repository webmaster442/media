// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Security.AccessControl;

using JKToolKit.Spectre.AutoCompletion.Completion;
using JKToolKit.Spectre.AutoCompletion.Integrations;

using Media.Commands;
using Media.Infrastructure;

var mainApp = new CommandApp<DefaultCommand>();

Terminal.EnableUTF8Output();

mainApp.Configure(config =>
{
    config.SetApplicationName("FFCmd");
    config.AddAutoCompletion(config => config.AddPowershell());

    config.AddCommand<Info>("info")
          .WithDescription("Get information about a media file");

    config.AddCommand<Cut>("cut")
          .WithDescription("Cut a file without reencoding");

    config.AddCommand<Play>("play")
          .WithDescription("Play a media file with mpv");

    config.AddBranch("cd", cd =>
    {
        cd.SetDescription("CD related commands");

        cd.AddCommand<CdList>("list")
            .WithDescription("List tracks on a CD");

        cd.AddCommand<CdRip>("rip")
            .WithDescription("Rip a CD to audio files");

        cd.AddCommand<CdEject>("eject")
            .WithDescription("Eject a CD from the drive");

        cd.AddCommand<CdClose>("close")
            .WithDescription("Close the CD drive");
    });

    config.AddBranch("convert", convert =>
    {
        //Note: when adding a new command also add it to the BachCompile class

        convert.SetDescription("Convert audio/video files");
        convert.AddCommand<ConvertToAlac>("alac")
               .WithDescription("Convert audio to Apple Lossless Audio Codec (ALAC)");

        convert.AddCommand<ConvertToFlac>("flac")
              .WithDescription("Convert audio to Free Lossless Audio Codec (FLAC)");

        convert.AddCommand<ConvertToM4a>("m4a")
              .WithDescription("Convert audio to MPEG-4 Audio (M4A)");

        convert.AddCommand<ConvertToMp3>("mp3")
              .WithDescription("Convert audio to MPEG-1 Audio Layer III (MP3)");

        convert.AddCommand<ConvertToCdWav>("cdwav")
              .WithDescription("Convert audio to CD WAV");

        convert.AddCommand<ConvertToDVDWav>("dvdwav")
               .WithDescription("Convert audio to DVD video compatible WAV");

        convert.AddCommand<ConvertToAc3>("ac3")
               .WithDescription("Convert audio to Dolby Digital AC-3");

        convert.AddCommand<ConvertContactSheet>("contactsheet")
              .WithDescription("Create a contact sheet from a video file");

        convert.AddCommand<ConvertNtscDvd>("dvd-ntsc")
              .WithDescription("Create an NTSC DVD compatible MPEG-2 file with AC-3 audio");

        convert.AddCommand<ConvertNtscDvd>("dvd-pal")
            .WithDescription("Create an PAL DVD compatible MPEG-2 file with AC-3 audio");
    });
    config.AddBranch("bach", bach =>
    {
        bach.SetDescription("Batch process audio/video files");

        bach.AddCommand<BachNew>("new")
            .WithAlias("create")
            .WithDescription("Create a new batch project file");

        bach.AddCommand<BachAdd>("add")
            .WithDescription("Add files to a batch project file");

        bach.AddCommand<BachRemove>("remove")
            .WithDescription("Remove files from a batch project file");

        bach.AddCommand<BachClear>("clear")
            .WithAlias("reset")
            .WithDescription("Clear all files from a batch project file");

        bach.AddBranch("set", set =>
        {
            set.AddCommand<BachSetOutputDir>("output")
               .WithDescription("Set the output directory for the batch project file");

            set.AddCommand<BachSetConversion>("conversion")
               .WithDescription("Set the output conversion for the batch project file");
        });

        bach.AddCommand<BachCompile>("compile")
            .WithDescription("Compile the batch project file to a shell script");
    });
    config.AddBranch("extract", extract =>
    {
        extract.SetDescription("Extract audio/video stream from files");

        extract.AddCommand<ExtractAudioStereoM4a>("stereo-m4a")
               .WithDescription("Extract adudio to a stereo (downmixed) m4a audio stream");

        extract.AddCommand<ExtractAudioCopy>("audio")
               .WithDescription("Extract audio stream without reencoding");
    });
    config.AddBranch("update", update =>
    {
        update.SetDescription("Update related commands");

        update.AddCommand<UpdateFFMpeg>("ffmpeg")
              .WithDescription("Update ffmpeg");

        update.AddCommand<UpdateMpv>("mpv")
              .WithDescription("Update mpv");

        update.AddCommand<UpdateYtdlp>("ytdlp")
              .WithDescription("Update youtube-dlp");

        update.AddCommand<UpdateAll>("all")
              .WithDescription("Update all tools");
    });
    config.AddBranch("mux", mux =>
    {
        mux.SetDescription("mux releated commands");

        mux.AddCommand<MuxAddAudio>("add-audio")
           .WithDescription("Add audio stream to a video file");

        mux.AddCommand<MuxAddSubtitle>("add-subtitle")
           .WithDescription("Add subtitle stream to a video file");
    });
});

await mainApp.RunAsync(args);

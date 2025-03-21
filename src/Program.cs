// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System;

using Media;
using Media.Commands;
using Media.Commands.Cd;
using Media.Commands.Config;
using Media.Commands.Convert;
using Media.Commands.Extract;
using Media.Commands.Info;
using Media.Commands.Mux;
using Media.Commands.Organize;
using Media.Commands.Playlist;
using Media.Commands.Presets;
using Media.Commands.Update;
using Media.Infrastructure;
using Media.Interop;
using Media.ShellAutoComplete.AutoComplete;
using Media.ShellAutoComplete.Integrations;

using var registar = ProgramFactory.CreateTypeRegistar();

await ProgramFactory.RunDatabaseJobsIfNeeded();

var mainApp = new CommandApp<DefaultCommand>(registar);

Terminal.EnableUTF8Output();

mainApp.Configure(config =>
{
    config.SetApplicationName("Media");
    config.PropagateExceptions();
    config.AddAutoCompletion(config => config.AddPowershell());

    config.AddCommand<Audio>("audio")
          .WithDescription("System audio settings");

    config.AddCommand<Cut>("cut")
          .WithDescription("Cut a file without reencoding");

    config.AddCommand<Sereve>("serve")
        .WithDescription("Start a DLNA server");

    config.AddCommand<ImgView>("imgview")
        .WithDescription("View images in a folder");

    config.AddCommand<Gui>("gui")
        .WithDescription("Start the graphical user interface");

    config.AddCommand<Install>("install")
        .WithDescription("Install the program. Create icons and add to path");

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

    config.AddBranch("config", cfg =>
    {
        cfg.SetDescription("Configuration related commands");

        cfg.AddCommand<ConfigList>("list")
            .WithDescription("List all configuration values");

        cfg.AddCommand<ConfigSet>("set")
            .WithDescription("Set a configuration value");
    });

    config.AddBranch("convert", convert =>
    {
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

    config.AddBranch("extract", extract =>
    {
        extract.SetDescription("Extract audio/video stream from files");

        extract.AddCommand<ExtractAudioStereoM4a>("stereo-m4a")
               .WithDescription("Extract adudio to a stereo (downmixed) m4a audio stream");

        extract.AddCommand<ExtractAudioCopy>("audio")
               .WithDescription("Extract audio stream without reencoding");
    });

    config.AddBranch("info", info =>
    {
        info.SetDescription("Information related commands");

        info.AddCommand<InfoFile>("file")
            .WithDescription("Get information about a media file");

        info.AddCommand<InfoEncoders>("encoders")
            .WithDescription("List available encoders");

        info.AddCommand<InfoHwEncoders>("hw-encoders")
            .WithDescription("List available hardware encoders");

        info.AddCommand<InfoDrives>("drives")
            .WithDescription("Print drive informations");

        info.AddCommand<Media.Commands.Info.Version>("version")
            .WithDescription("Print program version");

        info.AddCommand<Website>("website")
            .WithDescription("Open the project website");
    });

    config.AddBranch("mux", mux =>
    {
        mux.SetDescription("mux releated commands");

        mux.AddCommand<MuxAddAudio>("add-audio")
           .WithDescription("Add audio stream to a video file");

        mux.AddCommand<MuxAddSubtitle>("add-subtitle")
           .WithDescription("Add subtitle stream to a video file");
    });

    config.AddBranch("organize", org =>
    {
        org.SetDescription("File organization commands");

        org.AddCommand<CreateRules>("create")
            .WithDescription("Create an organization rule in a given folder");

        org.AddCommand<OrganizeByAbc>("abc")
            .WithDescription("Organize files to subdirs by alphabet");

        org.AddCommand<OrganizeByRule>("rule")
            .WithDescription("Organize files to subdirs by a rule file");
    });

    config.AddBranch("play", play =>
    {
        play.SetDescription("Play related commands");

        play.AddCommand<Play>("file")
            .WithDescription("Play a media file with mpv");

        play.AddCommand<PlayRandom>("random")
            .WithDescription("Play a random media file with mpv");
    });

    config.AddBranch("playlist", playlist =>
    {
        playlist.SetDescription("Playlist related commands");

        playlist.AddCommand<PlaylistNew>("new")
            .WithDescription("Create a new playlist");

        playlist.AddCommand<PlaylistAdd>("add")
            .WithDescription("Add a file to a playlist");

        playlist.AddCommand<PlaylistRemove>("remove")
            .WithDescription("Remove a file from a playlist");

        playlist.AddCommand<PlaylistClear>("clear")
            .WithDescription("Clear a playlist");

        playlist.AddCommand<PlaylistCopy>("copy")
            .WithDescription("Copy files from a playlist to a directory");
    });

    config.AddBranch("preset", presetconvert =>
    {
        presetconvert.AddCommand<ConvertDragDrop>("drop")
            .WithDescription("Convert multiple file using a drag & drop window");

        presetconvert.AddCommand<ListPresets>("list")
            .WithDescription("List available presets");

        presetconvert.AddCommand<ConvertPreset>("run")
            .WithDescription("Convert a file using a preset");
    });

    config.AddBranch("update", update =>
    {
        update.SetDescription("Update related commands");

        update.AddCommand<UpdateMedia>("media")
              .WithDescription("Update the media cli");

        update.AddCommand<UpdateFFMpeg>("ffmpeg")
              .WithDescription("Update ffmpeg");

        update.AddCommand<UpdateMpv>("mpv")
              .WithDescription("Update mpv");

        update.AddCommand<UpdateYtdlp>("ytdlp")
              .WithDescription("Update youtube-dlp");

        update.AddCommand<UpdateAll>("all")
              .WithDescription("Update all tools");
    });
});

try
{
    await mainApp.RunAsync(args);
}
catch (Exception e)
{
    if (e is CommandParseException
        or CommandRuntimeException
        or ToolDependencyException
        or OperationCanceledException)
    {
        Terminal.RedText(e.Message);
        Environment.Exit(ExitCodes.Error);
    }
    GlobalExceptionHandler.HandleExcpetion(e);
    Environment.Exit(ExitCodes.Exception);
}
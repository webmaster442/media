﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Infrastructure.Selector;
using Media.Infrastructure.SelectorItemProviders;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal sealed class Play : BasePlaylistCommand<Play.Settings>
{
    private readonly int _remotePort;
    private readonly Mpv _mpv;

    public class Settings : ValidatedCommandSettings
    {
        [Description("Input file")]
        [CommandArgument(0, "[input]")]
        [PathExists(AllowEmpty = true)]
        public string InputFile { get; init; }

        [Description("Use DLNA for browsing")]
        [CommandOption("-d|--dlna")]
        public bool Dlna { get; init; }

        [Description("Enable http remote control")]
        [CommandOption("-r|--remote")]
        public bool EnableRemote { get; init; }

        public Settings()
        {
            InputFile = string.Empty;
        }

        public override Spectre.Console.ValidationResult Validate()
        {
            if (Dlna && !string.IsNullOrEmpty(InputFile))
            {
                return Spectre.Console.ValidationResult.Error("DLNA and file input cannot be used together");
            }
            return base.Validate();
        }
    }

    public Play(ConfigAccessor configAccessor)
    {
        _remotePort = configAccessor.GetMpvRemotePort() ?? 12345;
        _mpv = new Mpv(configAccessor);
    }

    private async Task RunMpv(string fileName, bool enableRemote)
    {
        var pipeName = $"mpvsocket-{GetRandomId()}";
        var builder = new MpvCommandBuilder();

        if (enableRemote)
            builder.WithIpcServer(pipeName);

        builder.WithInputFile(fileName);

        using var process = _mpv.CreateProcess(builder.Build(),
                                               redirectStdIn: false,
                                               redirectStdOut: false,
                                               redirectStderr: false);

        process.Start();

        if (enableRemote)
        {
            var webapp = new MpvWebControllerApp(process.Id, _remotePort, pipeName);
            await webapp.RunAsync(CancellationToken.None);
        }
        Terminal.InfoText("Press a key to exit...");
        Console.ReadKey();
    }

    private static int GetRandomId()
        => Random.Shared.Next(100, 1000);

    private static async Task<string> DoDlnaBrowse()
    {
        using var consoleCancel = new ConsoleCancelTokenSource();
        using var provider = new DlnaItemProvider();

        var selector = new ItemSelector<DlnaItem, DlnaItemProvider.CurrentPath>(
            itemProvider: provider,
            title: "Select a media server",
            defaultPath: DlnaItemProvider.CurrentPath.Empty);

        var selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

        return selectedItem.Uri.ToString();
    }

    private static async Task<string> DoFileSelection()
    {
        using var consoleCancel = new ConsoleCancelTokenSource();

        var selector = new ItemSelector<Item, string>(
            itemProvider: new FileSystemItemProvider(Mpv.GetSupportedExtensions()),
            title: "Select a file",
            defaultPath: string.Empty);

        Item selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

        return $"\"{selectedItem.FullPath}\"";
    }

    private async Task<string> CreatePlaylist(string directoryName)
    {
        Playlist result = new();
        var file = Path.Combine(directoryName, Path.ChangeExtension(Path.GetFileName(directoryName), ".m3u"));
        result.AddRange(RandomSelectorProvider.ScanSupportedFiles(directoryName, false));
        await SaveToFile(result, file, true);
        return file;
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.InputFile))
        {
            string file = string.Empty;

            file = settings.Dlna
                ? await DoDlnaBrowse()
                : await DoFileSelection();

            await RunMpv(file, settings.EnableRemote);
        }
        else if (Directory.Exists(settings.InputFile))
        {
            string playlist = await CreatePlaylist(settings.InputFile);
            await RunMpv(playlist, settings.EnableRemote);
        }
        else
        {
            await RunMpv(settings.InputFile, settings.EnableRemote);
        }
    }
}

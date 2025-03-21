// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.DbAdapters;
using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.CommandAttributes;
using Media.Infrastructure.Selector;
using Media.Infrastructure.SelectorItemProviders;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

[Example("Play a random file from a directory", @"media play random d:\")]
[Example("Play random 15 filse from a directory", @"media play random d:\ 15")]
[Example("Play a random file from a directory with subdir selector", @"media play random d:\ --browse")]
internal sealed class PlayRandom : AsyncCommand<PlayRandom.Settings>
{
    private readonly Mpv _mpv;
    private readonly RandomSelectorProvider _randomSelectorProvider;
    private readonly PlayedFilesAdapter _documentStore;

    internal class Settings : ValidatedCommandSettings
    {
        [Description("Enable subdirectory selector")]
        [CommandOption("-b|--browse")]
        public bool SubDirectorySelection { get; init; }

        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;

        [Range(1, 100)]
        [Description("Number of files to select. In range of 1 to 100")]
        [CommandArgument(1, "[count]")]
        public int SelectionCount { get; set; } = 1;
    }

    internal record class DirectoryEntry(string Name, string Path);

    public PlayRandom(ConfigAdapter configAccessor, PlayedFilesAdapter documentStore)
    {
        _mpv = new Mpv(configAccessor);
        _randomSelectorProvider = new();
        _documentStore = documentStore;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var playedFiles = await _documentStore.GetPlayedFilesAsync();

        MpvCommandBuilder builder = new();

        if (settings.SubDirectorySelection)
        {
            using var consoleCancel = new ConsoleCancelTokenSource();

            var selector = new ItemSelector<Item, string>(
                itemProvider: _randomSelectorProvider,
                title: "Select a directory",
                defaultPath: settings.Folder);

            Item selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

            var files = RandomSelectorProvider.ScanSupportedFiles(selectedItem.FullPath)
                .Except(playedFiles)
                .OrderBy(_ => Random.Shared.Next())
                .Take(settings.SelectionCount);


            if (files.Any())
            {
                await _documentStore.AddPlayedFilesAsync(files);
                builder.WithInputFiles(files);
                _mpv.Start(builder);
            }
        }
        else
        {
            var files = RandomSelectorProvider.ScanSupportedFiles(settings.Folder)
                .Except(playedFiles)
                .OrderBy(_ => Random.Shared.Next())
                .Take(settings.SelectionCount)
                .ToArray();

            if (files.Any())
            {
                await _documentStore.AddPlayedFilesAsync(files);
                builder.WithInputFiles(files);
                _mpv.Start(builder);
            }
        }
        return ExitCodes.Success;
    }
}

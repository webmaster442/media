// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;
using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.Selector;
using Media.Infrastructure.SelectorItemProviders;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;
internal sealed class PlayRandom : AsyncCommand<PlayRandom.Settings>
{
    private readonly Mpv _mpv;
    private readonly RandomSelectorProvider _randomSelectorProvider;
    private readonly MediaDocumentStore _documentStore;

    internal class Settings : ValidatedCommandSettings
    {
        [Description("Enable subdirectory selector")]
        [CommandOption("-b|--browswe")]
        public bool SubDirectorySelection { get; init; }

        [DirectoryExists]
        [CommandArgument(0, "<folder>")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    internal record class DirectoryEntry(string Name, string Path);

    public PlayRandom(ConfigAccessor configAccessor, MediaDocumentStore documentStore)
    {
        _mpv = new Mpv(configAccessor);
        _randomSelectorProvider = new();
        _documentStore = documentStore;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await _documentStore.Init();

        MpvCommandBuilder builder = new();

        if (settings.SubDirectorySelection)
        {
            using var consoleCancel = new ConsoleCancelTokenSource();

            var selector = new ItemSelector<Item, string>(
                itemProvider: _randomSelectorProvider,
                title: "Select a directory",
                defaultPath: settings.Folder);

            Item selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

            var file = RandomSelectorProvider.ScanSupportedFiles(selectedItem.FullPath)
                .OrderBy(_ => Random.Shared.Next())
                .FirstOrDefault();


            if (!string.IsNullOrEmpty(file))
            {
                builder.WithInputFile(file);
                _mpv.Start(builder);
            }
        }
        else
        {
            var file = RandomSelectorProvider.ScanSupportedFiles(settings.Folder)
                .Where(file => !_documentStore.PlayedFiles.Contains(file))
                .OrderBy(_ => Random.Shared.Next())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(file))
            {
                _documentStore.PlayedFiles.Add(file);
                await _documentStore.Save();

                builder.WithInputFile(file);
                _mpv.Start(builder);
            }
        }
        return ExitCodes.Success;
    }
}

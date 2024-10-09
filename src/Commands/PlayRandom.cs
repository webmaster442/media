// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

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

    internal class Settings : ValidatedCommandSettings
    {
        [Description("Enable subdirectory selector")]
        [CommandOption("-b|--browswe")]
        public bool SubDirectorySelection { get; init; }

        [DirectoryExists]
        [CommandArgument(0, "[folder]")]
        [Description("The folder to play from")]
        public string Folder { get; set; } = Environment.CurrentDirectory;
    }

    internal record class DirectoryEntry(string Name, string Path);

    public PlayRandom(ConfigAccessor configAccessor)
    {
        _mpv = new Mpv(configAccessor);
        _randomSelectorProvider = new();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
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
                .OrderBy(_ => Random.Shared.Next())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(file))
            {
                builder.WithInputFile(file);
                _mpv.Start(builder);
            }
        }
        return ExitCodes.Success;
    }
}

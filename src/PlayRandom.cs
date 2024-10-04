// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.Selector;
using Media.Interop;

namespace Media;
internal sealed class PlayRandom : AsyncCommand<PlayRandom.Settings>
{
    private readonly Mpv _mpv;
    private readonly RandomSelectorProvider _randomSelectorProvider;

    internal class Settings : ValidatedCommandSettings
    {
        [Description("Enable subdirectory selector")]
        [CommandOption("-b|--browswe")]
        public bool SubDirectorySelection { get; init; }
    }

    internal record class DirectoryEntry(string Name, string Path);

    public PlayRandom(ConfigAccessor configAccessor)
    {
        _mpv = new Mpv(configAccessor);
        _randomSelectorProvider = new();
    }

    private void StartPlayer(string? file)
    {
        using var process = _mpv.CreateProcess($"\"{file}\"",
                               redirectStdIn: false,
                               redirectStdOut: false,
                               redirectStderr: false);
        process.Start();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (settings.SubDirectorySelection)
        {
            using var consoleCancel = new ConsoleCancelTokenSource();

            var selector = new ItemSelector<Item, string>(
                itemProvider: _randomSelectorProvider,
                title: "Select a directory",
                defaultPath: Environment.CurrentDirectory);

            Item selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

            var file = RandomSelectorProvider.ScanSupportedFiles(selectedItem.FullPath)
                .OrderBy(_ => Random.Shared.Next())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(file))
            {
                StartPlayer(file);
            }
        }
        else
        {
            var file = RandomSelectorProvider.ScanSupportedFiles(Environment.CurrentDirectory)
                .OrderBy(_ => Random.Shared.Next())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(file))
            {
                StartPlayer(file);
            }
        }
        return ExitCodes.Success;
    }
}

// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

using AudioSwitcher.AudioApi.CoreAudio;

using Media.Dto.Internals;
using Media.Infrastructure.Selector;

using Spectre.Console;

namespace Media.Infrastructure.SelectorItemProviders;

internal sealed class AudioSelectorProvider : IItemProvider<Item, Guid>
{
    private readonly IReadOnlyList<CoreAudioDevice> _devices;

    public AudioSelectorProvider(IReadOnlyList<CoreAudioDevice> devices)
    {
        _devices = devices;
    }

    string IItemProvider<Item, Guid>.ConvertItem(in Item item)
        => $"{item.Icon} {item.Name}";

    Task<IReadOnlyCollection<Item>> IItemProvider<Item, Guid>.GetItemsAsync(Guid currentPath, CancellationToken cancellationToken)
    {
        var results = _devices.Select(x => new Item
        {
            FullPath = x.Id.ToString(),
            Icon = Emoji.Known.Loudspeaker,
            Name = x.FullName
        });
        return Task.FromResult<IReadOnlyCollection<Item>>(results.ToList());
    }

    Guid IItemProvider<Item, Guid>.SelectCurrentPath(in Item item) 
        => new Guid(item.FullPath);

    bool IItemProvider<Item, Guid>.SelectionCanExit(in Item selectedItem) 
        => true;
}

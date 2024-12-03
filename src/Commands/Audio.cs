// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using AudioSwitcher.CoreAudio;

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.Selector;
using Media.Infrastructure.SelectorItemProviders;

using Spectre.Console;

namespace Media.Commands;
internal class Audio : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        using var controller = new CoreAudioController();
        var devices = controller.GetDevices()
            .Where(d => d.DeviceType == AudioSwitcher.AudioApi.DeviceType.Playback
                   && d.State == AudioSwitcher.AudioApi.DeviceState.Active)
            .ToList();

        var defaultDeviceGuid = devices.First(d => d.IsDefaultDevice).Id;

        using var consoleCancel = new ConsoleCancelTokenSource();

        var selector = new ItemSelector<Item, Guid>(
            itemProvider: new AudioSelectorProvider(devices),
            title: "Select default audio output",
            defaultPath: defaultDeviceGuid);

        var selection = await selector.SelectItemAsync(consoleCancel.Token);

        var newDefault = devices.First(d => d.Id == new Guid(selection.FullPath));

        
        AnsiConsole.MarkupLine($"[green]Setting default audio device to {newDefault.FullName}[/]");
        newDefault.SetAsDefault();

        return 0;
    }
}

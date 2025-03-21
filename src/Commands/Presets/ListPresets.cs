// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.CommandAttributes;

using Spectre.Console;

namespace Media.Commands.Presets;

[Example("List available presets", "media preset list")]
public class ListPresets : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {

        var loaded = await Media.Infrastructure.PresetProvider.LoadPresetsAsync();
        PrintPresets(loaded);
        return ExitCodes.Success;
    }

    private static void PrintPresets(Dictionary<string, Preset> presets)
    {
        var table = new Table();
        table.AddColumns("Name", "Description");
        foreach (var preset in presets.Values)
        {
            table.AddRow(preset.Name, preset.Description);
        }
    }
}

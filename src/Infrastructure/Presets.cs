// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

using Media.Dto;
using Media.Embedded;

namespace Media.Infrastructure;

internal static class Presets
{
    private static async Task<Stream> GetPresetStream()
    {
        var fileName = Path.Combine(AppContext.BaseDirectory, EmbeddedResources.Presets);
        if (!File.Exists(fileName))
        {
            await EmbeddedResources.ExtractAsync(EmbeddedResources.Presets);
        }
        return File.OpenRead(fileName);
    }

    public static async Task<Dictionary<string, Preset>> LoadPresetsAsync()
    {

        await using var stream = await GetPresetStream();
        XmlSerializer xs = new XmlSerializer(typeof(Preset[]), new XmlRootAttribute("Presets"));
        if (xs.Deserialize(stream) is Preset[] results)
        {
            return results.ToDictionary(p => p.Name, p => p);
        }
        throw new InvalidOperationException($"{EmbeddedResources.Presets} is not a valid preset file");
    }

    public static async Task<Preset[]> LoadPresetArray()
    {
        await using var stream = await GetPresetStream();
        XmlSerializer xs = new XmlSerializer(typeof(Preset[]), new XmlRootAttribute("Presets"));
        if (xs.Deserialize(stream) is Preset[] results)
        {
            return results;
        }
        throw new InvalidOperationException($"{EmbeddedResources.Presets} is not a valid preset file");
    }
}

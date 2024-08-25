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
    public static async Task<Dictionary<string, Preset>> LoadPresetsAsync()
    {
        var fileName = Path.Combine(AppContext.BaseDirectory, EmbeddedResources.Presets);
        if (!File.Exists(fileName))
        {
            await EmbeddedResources.ExtractAsync(EmbeddedResources.Presets);
        }
        await using var stream = File.OpenRead(fileName);
        XmlSerializer xs = new XmlSerializer(typeof(Preset[]), new XmlRootAttribute("Presets"));
        if (xs.Deserialize(stream) is Preset[] results)
        {
            return results.ToDictionary(p => p.Name, p => p);
        }
        throw new InvalidOperationException($"{fileName} is not a valid preset file");
    }
}

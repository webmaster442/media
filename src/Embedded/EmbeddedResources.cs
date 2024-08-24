// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Embedded;

internal static class EmbeddedResources
{
    public const string UpdatePS1 = "Update.ps1";
    public const string TestImage = "testimg.png";
    public const string Presets = "presets.xml";

    public static Stream GetFile(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return typeof(EmbeddedResources).Assembly.GetManifestResourceStream($"Media.Embedded.{name}")
            ?? throw new InvalidOperationException($"Resource {name} not found");
    }

    public static async Task Extract(string fileName)
    {
        var targetName = Path.Combine(AppContext.BaseDirectory, fileName);
        await using var soruce = GetFile(fileName);
        await using var target = File.Create(targetName);
        await soruce.CopyToAsync(target);
    }
}

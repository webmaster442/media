// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Validation;

namespace Media.Infrastructure;

internal class BasePlalistSettings : ValidatedCommandSettings
{
    [Description("Playlist file")]
    [CommandOption("-p|--playlist")]
    [Required]
    [FileExists]
    public string PlaylistName { get; set; }

    public BasePlalistSettings()
    {
        var files = Directory.GetFiles(Environment.CurrentDirectory, "*.m3u");
        PlaylistName = files.Length == 1 ? files[0] : string.Empty;
    }
}
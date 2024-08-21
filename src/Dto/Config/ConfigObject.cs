// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto.Config;

public sealed class ConfigObject
{
    public Dictionary<string, DateTimeOffset> Versions { get; set; }

    public EncoderInfos? EncoderInfoCache { get; set; }

    public ConfigObject()
    {
        Versions = new();
    }
}

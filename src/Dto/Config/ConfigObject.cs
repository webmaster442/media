// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Reflection;

namespace Media.Dto.Config;

public sealed class ConfigObject
{
    public Dictionary<string, string> Settings { get; }
    public Version Version { get; set; }

    public ConfigObject()
    {
        Settings = new Dictionary<string, string>();
        Version = Assembly.GetAssembly(typeof(ConfigObject))?.GetName().Version ?? new Version();
    }

    public void FillWithDefaults()
    {
        foreach (var defaultValue in ConfigKeys.CurrentVersionDefaults)
        {
            Settings.TryAdd(defaultValue.Key, defaultValue.Value);
        }
    }
}

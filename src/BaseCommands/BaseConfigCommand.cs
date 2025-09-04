// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Reflection;

using Media.DbAdapters;

using Spectre.Console;

namespace Media.BaseCommands;

internal abstract class BaseConfigCommand<TSettings> : Command<TSettings>
    where TSettings : CommandSettings
{
    protected readonly ConfigAdapter _configAdapter;
    private readonly PropertyInfo[] _configProperties;

    public BaseConfigCommand(ConfigAdapter configAdapter)
    {
        _configAdapter = configAdapter;
        _configProperties = _configAdapter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    protected Table GetPropertiesTable()
    {
        Table table = new();
        table.AddColumns("Property", "Value", "Description", "Type");

        foreach (var property in _configProperties)
        {
            var value = property.GetValue(_configAdapter);
            var description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
            var type = property.PropertyType.Name;

            table.AddRow(property.Name, value?.ToString() ?? "null", description, type.ToString());
        }

        return table;
    }

    protected bool TrySetProperty(string name, string value)
    {
        var toSet = _configProperties.FirstOrDefault(p => p.Name == name);
        if (toSet == null)
        {
            return false;
        }

        if (toSet.PropertyType == typeof(string))
        {
            toSet.SetValue(_configAdapter, value);
            return true;
        }

        try
        {
            var converted = Convert.ChangeType(value, toSet.PropertyType);
            toSet.SetValue(_configAdapter, converted);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

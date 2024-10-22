// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;

using Spectre.Console;
using Spectre.Console.Json;

namespace Media.Infrastructure;

internal static class Terminal
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static void DisplayTable<T>(IEnumerable<T> items)
    {
        var table = new Table();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        table.AddColumns(properties.Select(x => x.Name).ToArray());
        foreach (var item in items)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty).ToArray();
            table.AddRow(values);
        }
        AnsiConsole.Write(table);
    }

    public static void DisplayObject<T>(T obj) where T : class
    {
        var jsonString = JsonSerializer.Serialize<T>(obj, Options);
        var widget = new JsonText(jsonString);
        AnsiConsole.Write(widget);
    }

    public static void EnableUTF8Output()
        => Console.OutputEncoding = Encoding.UTF8;

    public static void GreenText(string str)
        => AnsiConsole.MarkupLineInterpolated($"[green]{str.EscapeMarkup()}[/]");

    public static void InfoText(string str)
        => AnsiConsole.MarkupLineInterpolated($"[yellow]{str.EscapeMarkup()}[/]");

    internal static void RedText(string str)
         => AnsiConsole.MarkupLineInterpolated($"[red]{str.EscapeMarkup()}[/]");

    internal static bool Confirm(string message)
        => AnsiConsole.Confirm(message);
}

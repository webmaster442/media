using System.Text.Json;
using System.Text.Json.Serialization;

using Spectre.Console;
using Spectre.Console.Json;

namespace FFCmd.Infrastructure;

internal static class Terminal
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static void DisplayObject<T>(T obj) where T : class
    {
        var jsonString = JsonSerializer.Serialize<T>(obj, Options);
        var widget = new JsonText(jsonString);
        AnsiConsole.Write(widget);
    }

    public static void DisplayException(Exception e)
    {
        AnsiConsole.WriteException(e);
    }
}

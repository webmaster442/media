// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console;

namespace Media;

internal static class GlobalExceptionHandler
{
    internal static void HandleExcpetion(Exception exception)
    {
        if (exception is OperationCanceledException)
        {
            DisplayWarning(exception.Message);
            return;
        }

        CreateCrashReport(exception);

        AnsiConsole.WriteException(exception);
        Environment.Exit(ExitCodes.Exception);
    }

    private static void DisplayWarning(string message)
        => AnsiConsole.MarkupLine($"[bold yellow]{message.EscapeMarkup()}[/]");

    private static void CreateCrashReport(Exception exception)
    {
        var time = DateTime.UtcNow;
        var report = new Dto.CrashReport
        {
            Time = time,
            ExceptionMessage = exception.Message,
            StackTrace = exception.StackTrace?.Split('\n') ?? Array.Empty<string>(),
            Source = exception.Source ?? "Unknown",
            StartArguments = Environment.GetCommandLineArgs(),
            WorkDirectory = Environment.CurrentDirectory,
            OsVersion = Environment.OSVersion.VersionString
        };
        try
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, $"media_crashreport_{time:yyyyMMdd_HHmmss}.json");
            using var stream = File.Create(fileName);
            JsonSerializer.Serialize<Dto.CrashReport>(stream, report, new JsonSerializerOptions { WriteIndented = true });
            DisplayWarning($"Crash report written to: {fileName}");
        }
        catch (IOException ex)
        {
            DisplayWarning($"Failed to write crash report: {ex.Message}");
        }
    }
}

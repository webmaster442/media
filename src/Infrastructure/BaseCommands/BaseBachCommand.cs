using System.Text.Json;

using FFCmd.Dto;

using Spectre.Console.Cli;

namespace FFCmd.Infrastructure.BaseCommands;

internal abstract class BaseBachCommand<T> : AsyncCommand<T> where T : ValidatedCommandSettings
{
    protected readonly JsonSerializerOptions _serializerOptions;

    protected BaseBachCommand()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
    }

    protected async Task<BachProject> LoadProject(string projectFile)
    {
        using var stream = File.OpenRead(projectFile);
        return await JsonSerializer.DeserializeAsync<BachProject>(stream, _serializerOptions)
            ?? throw new InvalidOperationException("Bach project error");
    }

    protected async Task SaveProject(string projectFile, BachProject project)
    {
        using var stream = File.Create(projectFile);
        await JsonSerializer.SerializeAsync(stream, project, _serializerOptions);
    }

    protected IEnumerable<string> GetFiles(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            yield break;
        }

        if (File.Exists(pattern))
        {
            yield return pattern;
        }

        string directory = Path.GetDirectoryName(pattern) ?? Directory.GetCurrentDirectory();
        string searchPattern = Path.GetFileName(pattern);

        directory = Environment.ExpandEnvironmentVariables(directory);

        foreach (var file in Directory.EnumerateFiles(directory, searchPattern))
        {
            yield return file;
        }
    }
}

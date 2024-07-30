// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;

namespace Media.Infrastructure.BaseCommands;

internal abstract class BaseBachCommand<T> : BaseFileWorkCommand<T> where T : ValidatedCommandSettings
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
}

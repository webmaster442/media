// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.BaseCommands;

internal abstract class BaseFileWorkCommand<T> : AsyncCommand<T> where T : ValidatedCommandSettings
{
    protected static IEnumerable<string> GetFiles(string pattern)
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

﻿using Media.Database;
using Media.Infrastructure;
using Media.Infrastructure.Validation;

namespace Media.Commands;

internal class MediaLibAdd : AsyncCommand<MediaLibAdd.Settings>
{
    private readonly MediaDbSerives _dbSerives;

    public class Settings : ValidatedCommandSettings
    {
        [DirectoryExists]
        [Description("Path to add to the library")]
        [CommandArgument(0, "<path>")]
        public string Path { get; set; } = string.Empty;

        [Description("Add subdirectories as well")]
        [CommandOption("-r|--recursive")]
        public bool Recursive { get; set; }
    }

    public MediaLibAdd(MediaDbSerives dbSerives)
    {
        _dbSerives = dbSerives;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        string[] files = Directory.GetFiles(settings.Path, "*.*", settings.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        await _dbSerives.AddFiles(files);
        return ExitCodes.Success;
    }
}

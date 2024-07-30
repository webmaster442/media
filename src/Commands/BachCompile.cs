﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interop;

using Microsoft.Extensions.DependencyInjection;

namespace Media.Commands;

internal sealed class BachCompile : BaseBachCommand<BachCompile.Settings>
{
    private readonly CommandApp _bachApp;
    private readonly Dictionary<string, string> _commandNamesAndExtensions;
    private readonly IDryRunResultAcceptor _dryRunResultAcceptor;

    public class Settings : BaseBachSettings
    {
    }

    public BachCompile()
    {
        var services = new ServiceCollection();

        _dryRunResultAcceptor = new DryRunResultAcceptor();

        services.AddSingleton(_dryRunResultAcceptor);
        var registar = new TypeRegistrar(services);

        _bachApp = new CommandApp(registar);
        _bachApp.Configure(c =>
        {
            c.AddCommand<ConvertToAlac>("alac");
            c.AddCommand<ConvertToFlac>("flac");
            c.AddCommand<ConvertToM4a>("m4a");
            c.AddCommand<ConvertToMp3>("mp3");
            c.AddCommand<ConvertToCdWav>("cdwav");
            c.AddCommand<ConvertContactSheet>("contactsheet");
        });

        _commandNamesAndExtensions = new()
        {
            { "alac", "m4a" },
            { "flac", "flac" },
            { "m4a", "m4a" },
            { "mp3", "mp3" },
            { "cdwav", "wav" },
            { "contactsheet", "jpg" }
        };
    }

    protected override async Task CoreTaskWithoutExcepionHandling(CommandContext context, Settings settings)
    {
        BachProject project = await LoadProject(settings.ProjectName);

        var directory = Path.GetDirectoryName(settings.ProjectName)
            ?? throw new InvalidOperationException("Couldn't get directory name");

        var scriptPath = Path.ChangeExtension(Path.GetFileNameWithoutExtension(settings.ProjectName), ".ps1");

        string[] cmdLine =
        [
            project.ConversionCommand, .. project.Args, "input", "output",
            ];

        if (!FFMpeg.TryGetInstalledPath(out string ffmpegPath))
        {
            throw new InvalidOperationException("FFMpeg not found.");
        }

        var builder = new PowershellBuilder()
            .WithUtf8Enabled()
            .WithWindowTitle(Path.GetFileNameWithoutExtension(settings.ProjectName))
            .WithClear();

        foreach (var inputFile in project.Files)
        {

            var fileName = Path.GetFileNameWithoutExtension(inputFile);
            var outputFile = Path.Combine(project.OutputDirectory, Path.ChangeExtension(fileName, _commandNamesAndExtensions[project.ConversionCommand]));
            cmdLine[^2] = inputFile;
            cmdLine[^1] = outputFile;
            _bachApp.Run(cmdLine);

            var generated = _dryRunResultAcceptor.Result;

            builder.WithCommandIfFileNotExists(outputFile, $"\"{ffmpegPath}\" {generated}");
        }

        using var scriptWriter = File.CreateText(Path.Combine(directory, scriptPath));

        await scriptWriter.WriteAsync(builder.Build());
    }
}

﻿using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace Media.Tests.System;
public abstract class FFMpegCommandSystemTest
{
    private CommandApp _testApp;
    private DryRunResultAcceptor _acceptor;
    private List<string> _mockedFiles;

    [SetUp]
    public void SetupBase()
    {
        _mockedFiles = new List<string>();
        var services = new ServiceCollection();

        _acceptor = new DryRunResultAcceptor
        {
            Enabled = false
        };

        services.AddSingleton<IDryRunResultAcceptor>(_acceptor);
        var registar = new TypeRegistrar(services);
        registar.Build();

        _testApp = new CommandApp(registar);

        Setup();
    }

    [TearDown]
    public void TearDownBase()
    {
        foreach (var file in _mockedFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
        TearDown();
    }

    protected virtual void TearDown()
    {
        //Empty for now
    }

    protected virtual void Setup()
    {
        //Empty for now
    }

    protected void MockExe(string exeName)
    {
        var sourceFile = Path.Combine(AppContext.BaseDirectory, "Media.MockExecutable.exe");
        var targetFile = Path.Combine(AppContext.BaseDirectory, exeName);
        File.Copy(sourceFile, targetFile, true);
    }

    public async Task<string[]> ReadMockExeStartArgs()
    {
        var sourceFile = Path.Combine(AppContext.BaseDirectory, "Media.MockExecutable.txt");
        return await File.ReadAllLinesAsync(sourceFile);
    }

    protected void SetCommand<TCommand>() where TCommand: class, ICommand
    {
        _testApp.SetDefaultCommand<TCommand>();
    }

    protected void MockFiles(params string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            _mockedFiles.Add(fullPath);
            if (!File.Exists(fullPath))
            {
                using var d = File.Create(fullPath);
            }
        }
    }

    protected async Task<int> ExecuteAsync(params string[] args)
    {
        var result = await _testApp.RunAsync(args);
        await Task.Delay(100); //wait for result writing to be completed
        return result;
    }
}
using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

namespace Media.Tests.System;
public abstract class SystemTestBase
{
    private CommandApp _testApp;
    private DryRunResultAcceptor _acceptor;
    private List<string> _mockedFiles;

    [SetUp]
    public void SetupBase()
    {
        _mockedFiles = new List<string>();
        var services = new ServiceCollection();

        _acceptor = new DryRunResultAcceptor();

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

    protected void SetCommand<TCommand>() where TCommand: class, ICommand
    {
        _testApp.SetDefaultCommand<TCommand>();
    }

    protected void MockFile(string fileName)
    {
        var fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
        _mockedFiles.Add(fullPath);
        if (!File.Exists(fullPath))
        {
            using var d = File.Create(fullPath);
        }
    }

    protected async Task<int> ExecuteAsync(params string[] args)
    {
        return await _testApp.RunAsync(args);
    }

    protected string GetResults()
    {
        return _acceptor.Result;
    }
}

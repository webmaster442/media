namespace Media.Tests.Autocomplete;

public sealed class CompletePowershellIntegration
{
    [Test]
    public void TestParseStartArgs_WithDllCommand_ShouldSetDotnetRuntime()
    {
        var startArgs = StartArgs.ParseStartArgs("myapp.dll");
        Assert.That(startArgs.Runtime, Is.EqualTo("dotnet"));
        Assert.That(startArgs.Command, Is.EqualTo("myapp.dll"));
    }

    [Test]
    public void TestParseStartArgs_WithDotnetCommand_ShouldSetCommandToSecondArg()
    {
        var startArgs = StartArgs.ParseStartArgs("dotnet", "myapp.dll");
        Assert.That(startArgs.Runtime, Is.EqualTo("dotnet"));
        Assert.That(startArgs.Command, Is.EqualTo("myapp.dll"));
    }

    [Test]
    public void TestParseStartArgs_WithNonDotnetCommand_ShouldSetCommandToFirstArg()
    {
        var startArgs = StartArgs.ParseStartArgs("myapp.exe");
        Assert.That(startArgs.Runtime, Is.EqualTo(string.Empty));
        Assert.That(startArgs.Command, Is.EqualTo("myapp.exe"));
    }


    // Assert that the integration builder does not throw for now

    [Test]
    public void TestBuildIntegration_WithNullStartArgs_ShouldUseSelfStartCommand()
    {
        Assert.DoesNotThrow(() =>
        {
            var settings = new PowershellIntegrationBuilderSettings { Install = false, StartArgs = null };
            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
        });
    }

    [Test]
    public void TestBuildIntegration_WithStartArgs_ShouldUseProvidedStartArgs()
    {
        Assert.DoesNotThrow(() =>
        {
            var startArgs = new StartArgs("dotnet", "myapp.dll");
            var settings = new PowershellIntegrationBuilderSettings { Install = false, StartArgs = startArgs };
            var result = PowershellIntegrationBuilder.BuildIntegration(settings);
        });
    }
}


using Media.Commands;

namespace Media.Tests.System;

[TestFixture]
internal class ConvertTests : SystemTestBase
{
    [Test]
    [Timeout(5000)]
    public async Task TestConvertToAlac()
    {
        SetCommand<ConvertToAlac>();
        MockFile("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "output.m4a");

        Assert.That(exitCode, Is.EqualTo(0));
        var results = GetResults();

        Assert.That(results, Is.EqualTo("-i \"input.wav\" -vn -c:a alac \"output.m4a\""));
    }
}

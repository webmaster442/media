using Media.Commands;

namespace Media.Tests.System;

[TestFixture]
[Timeout(5000)]
internal class ConvertTests : FFMpegCommandSystemTest
{
    [Test]
    public async Task TestConvertToAlac()
    {
        SetCommand<ConvertToAlac>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "output.m4a");

        Assert.That(exitCode, Is.EqualTo(0));
        var results = GetResults();

        Assert.That(results, Is.EqualTo("-i \"input.wav\" -vn -c:a alac \"output.m4a\""));
    }

    [Test]
    public async Task TestConvertToFlac()
    {
        SetCommand<ConvertToFlac>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "-c", "5", "output.flac");

        Assert.That(exitCode, Is.EqualTo(0));
        var results = GetResults();

        Assert.That(results, Is.EqualTo("-i \"input.wav\" -vn -compression_level 5 -c:a flac \"output.flac\""));
    }

    [Test]
    public async Task TestConvertToAc3()
    {
        SetCommand<ConvertToAc3>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "-b", "128k", "output.ac3");

        Assert.That(exitCode, Is.EqualTo(0));
        var results = GetResults();

        Assert.That(results, Is.EqualTo("-i \"input.wav\" -vn -c:a ac3 -b:a 128k \"output.ac3\""));
    }
}

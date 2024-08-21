// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Commands;

namespace Media.Tests.System;

[TestFixture]
[CancelAfter(5000)]
internal class ConvertTests : FFMpegCommandSystemTest
{
    protected override void Setup()
    {
        MockExe("ffmpeg.exe");
        MockFiles("input.wav");
    }

    [Test]
    public async Task TestConvertToAlac()
    {
        SetCommand<ConvertToAlac>();

        int exitCode = await ExecuteAsync("input.wav", "output.m4a");

        Assert.That(exitCode, Is.EqualTo(0));
        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-c:a", "alac", "output.m4a"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToFlac()
    {
        SetCommand<ConvertToFlac>();

        int exitCode = await ExecuteAsync("input.wav", "-c", "5", "output.flac");

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-compression_level", "5", "-c:a", "flac", "output.flac"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToAc3()
    {
        SetCommand<ConvertToAc3>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "-b", "128k", "output.ac3");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-c:a", "ac3", "-b:a", "128k", "output.ac3"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToM4a()
    {
        SetCommand<ConvertToM4a>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "-b", "128k", "output.m4a");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-c:a", "aac", "-b:a", "128k", "output.m4a"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToMp3()
    {
        SetCommand<ConvertToMp3>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "-b", "128k", "output.mp3");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-b:a", "128k", "output.mp3"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToCdWav()
    {
        SetCommand<ConvertToCdWav>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "output.wav");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-c:a", "pcm_s16le", "-ar", "44100", "output.wav"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertToDVDWav()
    {
        SetCommand<ConvertToDVDWav>();
        MockFiles("input.wav");

        int exitCode = await ExecuteAsync("input.wav", "output.wav");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.wav", "-vn", "-c:a", "pcm_s16le", "-ar", "48000", "output.wav"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertNtscDvd()
    {
        SetCommand<ConvertNtscDvd>();
        MockFiles("input.avi");

        int exitCode = await ExecuteAsync("input.avi", "-b", "320k", "output.mpg");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.avi", "-c:a", "ac3", "-b:a", "320k", "-target", "ntsc-dvd", "-aspect", "16:9", "output.mpg"];

        Assert.That(results, Is.EqualTo(expected));
    }

    [Test]
    public async Task TestConvertPalDvd()
    {
        SetCommand<ConvertPalDvd>();
        MockFiles("input.avi");

        int exitCode = await ExecuteAsync("input.avi", "-b", "320k", "output.mpg");

        Assert.That(exitCode, Is.EqualTo(0));

        var results = await ReadMockExeStartArgs();

        string[] expected = ["-i", "input.avi", "-c:a", "ac3", "-b:a", "320k", "-target", "pal-dvd", "-aspect", "16:9", "output.mpg"];

        Assert.That(results, Is.EqualTo(expected));
    }
}

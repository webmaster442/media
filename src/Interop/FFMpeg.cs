// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;

namespace Media.Interop;

internal sealed class FFMpeg : InteropBase
{
    public static class AudioCodecNames
    {
        public const string Alac = "alac";
        public const string PcmS16Le = "pcm_s16le";
        public const string Aac = "aac";
        public const string Copy = "copy";
        public const string Ac3 = "ac3";
        public const string Flac = "flac";
    }

    public static class VideoCodecNames
    {
        public const string Copy = "copy";
    }

    public static class TargetNames
    {
        public const string NtscDvd = "ntsc-dvd";
        public const string PalDvd = "pal-dvd";
    }

    private const string FfmpegBinary = "ffmpeg.exe";
    private readonly ConfigAccessor _configAccessor;

    public FFMpeg(ConfigAccessor configAccessor) : base(FfmpegBinary)
    {
        _configAccessor = configAccessor;
    }

    public string GetEnoderList()
    {
        using var ffmpegProcess = CreateProcess("-hide_banner -encoders", false, true, false);
        ffmpegProcess.Start();
        return ffmpegProcess.StandardOutput.ReadToEnd();
    }

    protected override string? GetExternalPath()
        => _configAccessor.GetExternalFFMpegPath();
}

// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.BaseCommands;
using Media.Interfaces;
using Media.Interop;

namespace Media.Commands;

[Example("Extract audio stream from a video file without reencoding", "media extract audio input.mkv output.ac3")]
[Example("Extract the 2nd audio stream from a video file without reencoding", "media extract audio input.mkv output.ac3 -a 1")]
internal sealed class ExtractAudioCopy : BaseFFMpegCommand<ExtractAudioCopy.Settings>
{
    public ExtractAudioCopy(ConfigAccessor configAccessor, IDryRunResultAcceptor dryRunResultAcceptor)
        : base(configAccessor, dryRunResultAcceptor)
    {
    }

    public class Settings : BaseFFMpegSettings
    {
        [Range(0, 999)]
        [Description("Audio stream index")]
        [CommandOption("-a|--audio-stream")]
        public int AudioStreamIndex { get; set; } = 0;
    }

    protected override void BuildCommandLine(FFMpegCommandBuilder builder, Settings settings)
    {
        builder
            .WithInputFile(settings.InputFile)
            .WithOutputFile(settings.OutputFile)
            .IgnoreVideo()
            .WithAudioStreamSelection(settings.AudioStreamIndex)
            .WithAudioCodec(FFMpeg.AudioCodecNames.Copy);
    }
}
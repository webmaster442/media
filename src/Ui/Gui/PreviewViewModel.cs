// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;

using Media.DbAdapters;
using Media.Interop;

namespace Media.Ui.Gui;

internal sealed partial class PreviewViewModel : ObservableObject
{
    private readonly FFMpeg _ffMpeg;
    private readonly string _filePath;
    private readonly bool _isFfmpegInstalled;

    private string GetThumbNailPath(string path)
    {
        static ulong CalculateFnvHash(string input)
        {
            const ulong fnvPrime = 1099511628211;
            ulong hash = 14695981039346656037;
            foreach (var c in input)
            {
                hash ^= c;
                hash *= fnvPrime;
            }
            return hash;
        }

        static string Base26Encode(ulong input)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = new StringBuilder();
            do
            {
                result.Append(alphabet[(int)(input % 26)]);
                input /= 26;
            }
            while (input > 0);
            return result.ToString();
        }

        var drive = Path.GetPathRoot(path) ?? throw new InvalidOperationException("Drive can't be determined");
        var thumbnailId = Base26Encode(CalculateFnvHash(path));
        var mediaFolder = Path.Combine(drive, ".media");
        if (!Directory.Exists(mediaFolder))
        {
            Directory.CreateDirectory(mediaFolder);
        }
        return Path.Combine(mediaFolder, $"{thumbnailId}.mp4");
    }

    public PreviewViewModel(ConfigAdapter configAdapter, string filePath)
    {
        MediaSource = new Uri("file://");
        _ffMpeg = new FFMpeg(configAdapter);
        _filePath = filePath;
        _isFfmpegInstalled = _ffMpeg.TryGetInstalledPath(out _);
    }

    [ObservableProperty]
    public partial Uri MediaSource { get; private set; }

    public async Task Initialize()
    {
        var thumbnailPath = GetThumbNailPath(_filePath);
        if (File.Exists(thumbnailPath))
        {
            MediaSource = new Uri($"file://{thumbnailPath}");
        }
        else if (_isFfmpegInstalled)
        {
            var totalTime = await FFProbe.GetDurationInSeconds(_filePath);

            int starttime = (int)Math.Ceiling(totalTime * 0.1);

            FFMpegCommandBuilder builder = new();
            var cli = builder.WithInputFile(_filePath)
                .WithOutputFile(thumbnailPath)
                .WithStartTimeInSeconds(starttime)
                .WithDurationInSeconds(30)
                .WithIgnoreAudio()
                .WithAcceleration("d3d11va")
                .WithVideoCodec("libx264")
                .WithVideoQuality(10)
                .WithVideoPreset("fast")
                .WithVideoFilter("fps='min(15, source_fps)',scale='min(480,iw)':min'(480,ih)':force_original_aspect_ratio=decrease")
                .Build();

            using var ffmpegProcess = _ffMpeg.CreateProcess(cli, false, false, false);
            ffmpegProcess.Start();
            await ffmpegProcess.WaitForExitAsync();

            MediaSource = new Uri($"file://{thumbnailPath}");
        }
        else
        {
            MediaSource = new Uri($"file://");
        }
    }
}

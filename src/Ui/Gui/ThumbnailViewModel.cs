// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Media.Interop;

namespace Media.Ui.Gui;

internal sealed partial class ThumbnailViewModel : ObservableObject
{
    private readonly FFMpeg _ffMpeg;
    private readonly bool _isFfmpegInstalled;

    private string GetThumbNailPath(string path)
    {
        static ulong CalculateFnvHash(string input)
        {
            const ulong fnvPrime = 1099511628211;
            const ulong fnvOffset = 14695981039346656037;
            ulong hash = fnvOffset;
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
        return Path.Combine(drive, ".media", $"{thumbnailId}.mp4");
    }

    public sealed class DisplayThumbnailMessage
    {
        public required string FullPath { get; init; }
    }

    public ThumbnailViewModel(FFMpeg ffMpeg)
    {
        WeakReferenceMessenger.Default.Register<DisplayThumbnailMessage>(this, OnDisplayThumbNail);
        ThumbnailPath = string.Empty;
        _ffMpeg = ffMpeg;
        _isFfmpegInstalled = _ffMpeg.TryGetInstalledPath(out _);
    }

    [ObservableProperty]
    public partial string ThumbnailPath { get; private set; }

    private async void OnDisplayThumbNail(object recipient, DisplayThumbnailMessage message)
    {
        var thumbnailPath = GetThumbNailPath(message.FullPath);
        if (File.Exists(thumbnailPath))
        {
            ThumbnailPath = thumbnailPath;
        }
        else if (_isFfmpegInstalled)
        {
            var totalTime = await FFProbe.GetDurationInSeconds(message.FullPath);
            
            int starttime = (int)Math.Ceiling(totalTime * 0.1);

            FFMpegCommandBuilder builder = new();
            var cli = builder.WithInputFile(message.FullPath)
                .WithOutputFile(thumbnailPath)
                .WithStartTimeInSeconds(starttime)
                .WithDurationInSeconds(60)
                .WithIgnoreAudio()
                .WithVideoCodec("libx264")
                .WithVideoQuality(7)
                .WithVideoPreset("fast")
                .WithVideoFilter("fps='min(15, source_fps)',scale='min(480,iw)':min'(480,ih)':force_original_aspect_ratio=decrease")
                .Build();

            using var ffmpegProcess = _ffMpeg.CreateProcess(cli, false, false, false);
            ffmpegProcess.Start();
            await ffmpegProcess.WaitForExitAsync();

            ThumbnailPath = thumbnailPath;
        }
        else
        {
            ThumbnailPath = string.Empty;
        }
    }
}

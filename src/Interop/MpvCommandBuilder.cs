// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Interfaces;

namespace Media.Interop;

internal class MpvCommandBuilder : IBuilder<string>
{
    private enum ArgumentPriority
    {
        CacheSeconds = 0,
        IpcServer = 1,
        InputFile = int.MaxValue,
    }

    private readonly Dictionary<ArgumentPriority, string> _data;

    private void SetArgument(ArgumentPriority argument, FormattableString value)
    {
        _data[argument] = value.ToString();
    }

    public MpvCommandBuilder()
    {
        _data = [];
    }

    public MpvCommandBuilder New()
    {
        _data.Clear();
        return this;
    }

    public MpvCommandBuilder WithIpcServer(string server)
    {
        SetArgument(ArgumentPriority.IpcServer, $"--input-ipc-server=\\\\.\\pipe\\{server}");
        return this;
    }

    public MpvCommandBuilder WithCacheSeconds(int seconds)
    {
        SetArgument(ArgumentPriority.CacheSeconds, $"--cache-secs={seconds}");
        return this;
    }

    public MpvCommandBuilder WithInputFile(string inputFile)
    {
        if (IsNetworkStream(inputFile))
        {
            SetArgument(ArgumentPriority.CacheSeconds, $"--cache-secs=30");
        }
        SetArgument(ArgumentPriority.InputFile, $"\"{inputFile}\"");
        return this;
    }

    public MpvCommandBuilder WithInputFiles(IEnumerable<string> inputFiles)
    {
        if (inputFiles.Any(f => IsNetworkStream(f)))
        {
            SetArgument(ArgumentPriority.CacheSeconds, $"--cache-secs=30");
        }

        SetArgument(ArgumentPriority.InputFile, $"{string.Join(' ', inputFiles.Select(f => $"\"{f}\""))}");
        return this;
    }

    private static bool IsNetworkStream(string inputFile)
    {
        return inputFile.StartsWith("http://")
            || inputFile.StartsWith("https://")
            || inputFile.StartsWith("ftp://")
            || inputFile.StartsWith("\\\\");
    }

    public string Build()
    {
        var ordered = _data.Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

        return string.Join(" ", ordered);
    }
}

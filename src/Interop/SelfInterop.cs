using System.Diagnostics;

namespace Media.Interop;

internal static class SelfInterop
{
    private static void RunMedia(string args)
    {
        using var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppContext.BaseDirectory, "media.exe"),
                Arguments = args,
                UseShellExecute = false
            }
        };
        p.Start();
    }

    public static void Play(string file)
        => RunMedia($"play file \"{file}\" -r");
}

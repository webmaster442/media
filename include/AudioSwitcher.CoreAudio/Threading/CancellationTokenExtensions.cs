using System.Diagnostics;
using System.Threading;

namespace AudioSwitcher.AudioApi.CoreAudio.Threading
{
    public static class CancellationTokenExtensions
    {

        public static void CancelAfter(this CancellationTokenSource source, int milliseconds)
        {
            using var timer = new Timer(state =>
            {
                var cts = state as CancellationTokenSource ?? throw new UnreachableException();

                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }

            }, source, -1, -1);

            timer.Change(-1, milliseconds);
        }

    }
}

// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

namespace Media.Infrastructure;

internal sealed class ConsoleCancelTokenSource : IDisposable
{
    private readonly CancellationTokenSource _tokenSource;

    public ConsoleCancelTokenSource()
    {
        _tokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += HandleCancelPress;
    }

    public CancellationToken Token => _tokenSource.Token;

    public void Dispose()
    {
        Console.CancelKeyPress -= HandleCancelPress;
        _tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public void ThrowIfCancellationRequested()
    {
        _tokenSource.Token.ThrowIfCancellationRequested();
    }

    private void HandleCancelPress(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        _tokenSource.Cancel();
    }
}

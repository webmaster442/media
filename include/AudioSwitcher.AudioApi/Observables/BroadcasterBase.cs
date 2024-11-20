namespace AudioSwitcher.AudioApi.Observables;

public abstract class BroadcasterBase<T> : IBroadcaster<T>, IDisposable
{
    protected bool _isDisposed;

    public abstract bool HasObservers { get; }

    public bool IsDisposed => _isDisposed;

    public abstract bool IsComplete { get; }

    public abstract void OnNext(T value);

    public abstract void OnError(Exception error);

    public abstract void OnCompleted();

    public abstract IDisposable Subscribe(IObserver<T> observer);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void Dispose(bool disposing);
}
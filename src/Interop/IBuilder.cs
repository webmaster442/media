namespace Media.Interop;

internal interface IBuilder<out T>
{
    T Build();
}

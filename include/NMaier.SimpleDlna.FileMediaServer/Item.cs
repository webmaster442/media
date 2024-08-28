namespace NMaier.SimpleDlna.FileMediaServer;

internal sealed partial class BackgroundCacher
{
    private struct Item
    {
        public readonly WeakReference File;

        public readonly WeakReference Store;

        public Item(WeakReference store, WeakReference file)
        {
            Store = store;
            File = file;
        }
    }
}

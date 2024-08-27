namespace NMaier.SimpleDlna.Server.Interfaces;

public interface IMediaFolder : IMediaItem
{
    int ChildCount { get; }

    int FullChildCount { get; }

    IEnumerable<IMediaFolder> ChildFolders { get; }

    IEnumerable<IMediaResource> ChildItems { get; }

    IMediaFolder? Parent { get; set; }

    void AddResource(IMediaResource res);

    void Cleanup();

    bool RemoveResource(IMediaResource res);

    void Sort(IComparer<IMediaItem> sortComparer, bool descending);
}
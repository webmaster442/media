using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Utilities;

namespace NMaier.SimpleDlna.Server.Types;

public class VirtualFolder : IMediaFolder
{
    private static readonly StringComparer Comparer =
      new NaturalStringComparer(true);

    private readonly List<IMediaFolder> _merged = new List<IMediaFolder>();
    private string? _comparableTitle;

    protected List<IMediaFolder> Folders = new List<IMediaFolder>();

    private string? _path;

    protected List<IMediaResource> Resources = new List<IMediaResource>();

    public VirtualFolder()
    {

    }

    public VirtualFolder(IMediaFolder? parent, string name)
      : this(parent, name, name)
    {
    }

    public VirtualFolder(IMediaFolder? parent, string name, string id)
    {
        Parent = parent;
        Id = id;
        Name = name;
    }

    public IEnumerable<IMediaResource> AllItems
    {
        get
        {
            return Folders.SelectMany(f => ((VirtualFolder)f).AllItems).
              Concat(Resources);
        }
    }

    public string Name { get; set; }

    public int ChildCount => Folders.Count + Resources.Count;

    public int FullChildCount => Resources.Count + (from f in Folders select f.FullChildCount).Sum();

    public IEnumerable<IMediaFolder> ChildFolders => Folders;

    public IEnumerable<IMediaResource> ChildItems => Resources;

    public string Id { get; set; }

    public IMediaFolder? Parent { get; set; }

    public virtual string Path
    {
        get
        {
            if (!string.IsNullOrEmpty(_path))
            {
                return _path;
            }
            var p = string.IsNullOrEmpty(Id) ? Name : Id;
            if (Parent != null)
            {
                var vp = Parent as VirtualFolder;
                _path = $"{(vp != null ? vp.Path : Parent.Id)}/v:{p}";
            }
            else
            {
                _path = p;
            }
            return _path;
        }
    }

    public IHeaders Properties
    {
        get
        {
            var rv = new RawHeaders { { "Title", Title } };
            return rv;
        }
    }

    public virtual string Title => Name;

    public void AddResource(IMediaResource res)
    {
        Resources.Add(res);
    }

    public virtual void Cleanup()
    {
        foreach (var m in _merged)
        {
            m.Cleanup();
        }
        foreach (var f in Folders.ToList())
        {
            f.Cleanup();
        }
        if (ChildCount != 0)
        {
            return;
        }
        var vp = Parent as VirtualFolder;
        vp?.ReleaseFolder(this);
    }

    public int CompareTo(IMediaItem? other)
    {
        if (other == null)
        {
            return 1;
        }
        return Comparer.Compare(Title, other.Title);
    }

    public bool Equals(IMediaItem? other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }
        return Title.Equals(other.Title);
    }

    public bool RemoveResource(IMediaResource res)
    {
        return Resources.Remove(res);
    }

    public void Sort(IComparer<IMediaItem> sortComparer, bool descending)
    {
        foreach (var f in Folders)
        {
            f.Sort(sortComparer, descending);
        }
        Folders.Sort(sortComparer);
        Resources.Sort(sortComparer);
        if (descending)
        {
            Folders.Reverse();
            Resources.Reverse();
        }
    }

    public string ToComparableTitle()
    {
        return _comparableTitle ?? (_comparableTitle = Title.StemCompareBase());
    }

    public void AdoptFolder(IMediaFolder folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }
        var vf = folder.Parent as VirtualFolder;
        vf?.ReleaseFolder(folder);
        folder.Parent = this;
        if (!Folders.Contains(folder))
        {
            Folders.Add(folder);
        }
    }

    public void Merge(IMediaFolder folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder));
        }
        _merged.Add(folder);
        foreach (var item in folder.ChildItems)
        {
            AddResource(item);
        }
        foreach (var cf in folder.ChildFolders)
        {
            var ownFolder = (from f in Folders
                             where f is VirtualFolder && f.Title == cf.Title
                             select f as VirtualFolder
              ).FirstOrDefault();
            if (ownFolder == null)
            {
                ownFolder = new VirtualFolder(this, cf.Title, cf.Id);
                AdoptFolder(ownFolder);
            }
            ownFolder.Merge(cf);
        }
    }

    public void ReleaseFolder(IMediaFolder folder)
    {
        Folders.Remove(folder);
    }
}

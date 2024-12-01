using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.FileMediaServer.Files;
using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;
using NMaier.SimpleDlna.Server.Utilities;

using Timer = System.Timers.Timer;

namespace NMaier.SimpleDlna.FileMediaServer;

public sealed class FileServer
: Logging, IMediaServer, IVolatileMediaServer, IDisposable
{
    private static readonly Random random = new Random();

    private static readonly StringComparer icomparer =
      StringComparer.CurrentCultureIgnoreCase;

    private static readonly double changeDefaultTime =
      TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static readonly double changeRenamedTime =
      TimeSpan.FromSeconds(10).TotalMilliseconds;

    private static readonly double changeDeleteTime =
      TimeSpan.FromSeconds(2).TotalMilliseconds;

    private readonly Timer changeTimer =
      new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);

    private readonly DirectoryInfo[] directories;

    private readonly Identifiers ids;
    private readonly Lock _lock = new(); 

    private readonly Regex regSanitizeExt =
      new Regex(@"[^\w\d]+", RegexOptions.Compiled);

    private readonly DlnaMediaTypes types;

    private readonly FileSystemWatcher[] watchers;

    private readonly Timer watchTimer =
      new Timer(TimeSpan.FromMinutes(random.Next(27, 33)).TotalMilliseconds);

    private bool isRescanning;

    private DateTime lastChanged = DateTime.Now;

    private readonly ConcurrentQueue<WeakReference> pendingFiles = new ConcurrentQueue<WeakReference>();

    private bool rescanning = true;

    private readonly object _lockObject = new object();

    // FileStore store;

    public FileServer(DlnaMediaTypes types, Identifiers ids, ILoggerFactory loggerFactory,
      params DirectoryInfo[] directories) : base(loggerFactory)
    {
        this.types = types;
        this.ids = ids;
        this.directories = directories.Distinct().ToArray();
        Filter = new ExtensionFilter(this.types.GetExtensions());

        if (this.directories.Length == 0)
        {
            throw new ArgumentException(
              "Provide one or more directories",
              nameof(directories)
              );
        }
        var parent = this.directories[0].Parent ?? this.directories[0];
        FriendlyName = this.directories.Length == 1
          ? $"{this.directories[0].Name} ({parent.FullName})"
          : $"{this.directories[0].Name} ({parent.FullName}) + {this.directories.Length - 1}";
        watchers = (from d in directories
                    select new FileSystemWatcher(d.FullName)).ToArray();
        UUID = DeriveUUID();
    }

    internal ExtensionFilter Filter { get; }

    public void Dispose()
    {
        foreach (var w in watchers)
        {
            w.Dispose();
        }
        changeTimer?.Dispose();
        watchTimer?.Dispose();
        //store?.Dispose();
        FileStreamCache.Clear();
    }

    public IHttpAuthorizationMethod? Authorizer { get; set; }

    public string FriendlyName { get; set; }

    // ReSharper disable once MemberInitializerValueIgnored
    public Guid UUID { get; } = Guid.NewGuid();
    
    public IMediaItem GetItem(string id)
    {
        lock (ids)
        {
            return ids.GetItemById(id) 
                ?? throw new InvalidOperationException();
        }
    }

    public event EventHandler? Changed;

    public bool Rescanning
    {
        get { return rescanning; }
        set
        {
            if (rescanning == value)
            {
                return;
            }
            rescanning = value;
            if (rescanning)
            {
                Rescan();
            }
        }
    }

    public void Rescan()
    {
        RescanInternal();
    }

    public event EventHandler? Changing;

    private Guid DeriveUUID()
    {
        var bytes = Guid.NewGuid().ToByteArray();
        var i = 0;
        var copy = Encoding.ASCII.GetBytes("sdlnafs");
        for (; i < copy.Length; ++i)
        {
            bytes[i] = copy[i];
        }
        copy = Encoding.UTF8.GetBytes(FriendlyName);
        for (var j = 0; j < copy.Length && i < bytes.Length - 1; ++i, ++j)
        {
            bytes[i] = copy[j];
        }
        return new Guid(bytes);
    }

    private void DoRoot()
    {
        IMediaFolder newMaster;
        if (directories.Length == 1)
        {
            newMaster = new PlainRootFolder(this, directories[0]);
        }
        else
        {
            var virtualMaster = new VirtualFolder(
              null,
              FriendlyName,
              Identifiers.GENERAL_ROOT
              );
            foreach (var d in directories)
            {
                virtualMaster.Merge(new PlainRootFolder(this, d));
            }
            newMaster = virtualMaster;
        }
        RegisterNewMaster(newMaster);
    }

    private bool HandleFileAdded(string fullPath)
    {
        lock (_lock)
        {
            FileInfo info = new FileInfo(fullPath);
            if (ids.GetItemByPath(info.FullName) is IMediaResource item)
            {
                Logger.LogDebug("Did find an existing {filename}", info.FullName);
            }
            if (ids.GetItemByPath(info.Directory?.FullName ?? "") is not PlainFolder folder)
            {
                Logger.LogDebug("Did not find folder for {directoryname}", info.Directory?.FullName);
                return false;
            }
            item = GetFile(folder, info);
            if (item == null)
            {
                Logger.LogDebug("Failed to create new item for {folder} - {info}", folder.Path, info.FullName);
                return false;
            }
            if (!Allowed(item))
            {
                return true;
            }
            folder.AddResource(item);
            Logger.LogDebug("Added {path} to corpus", item.Path);
            return true;
        }
    }

    private bool HandleFileDeleted(string fullPath)
    {
        lock (_lock)
        {
            var info = new FileInfo(fullPath);
            if (ids.GetItemByPath(info.FullName) is not IMediaResource item
                || ids.GetItemByPath(info.Directory?.FullName ?? "") is not VirtualFolder folder)
            {
                return false;
            }
            return folder.RemoveResource(item);
        }
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        try
        {
            var ext = string.Empty;
            if (!string.IsNullOrEmpty(e.FullPath))
            {
                ext = Path.GetExtension(e.FullPath);
                ext = string.IsNullOrEmpty(ext) ? string.Empty : ext.Substring(1);
            }
            if (!Filter.Filtered(ext))
            {
                Logger.LogDebug(
                  "Skipping name {name} {extension}",
                  e.Name, Path.GetExtension(e.FullPath));
                return;
            }
            Logger.LogDebug(
              "File System changed ({fullpath}): {changetype}", e.FullPath, e.ChangeType);
            lock (_lock)
            {
                var master = ids.GetItemById(Identifiers.GENERAL_ROOT) as VirtualFolder;
                if (master != null)
                {
                    switch (e.ChangeType)
                    {
                        case WatcherChangeTypes.Changed:
                            if (HandleFileDeleted(e.FullPath) && HandleFileAdded(e.FullPath))
                            {
                                ReaddRoot(master);
                                return;
                            }
                            break;

                        case WatcherChangeTypes.Created:
                            if (HandleFileAdded(e.FullPath))
                            {
                                ReaddRoot(master);
                                return;
                            }
                            break;

                        case WatcherChangeTypes.Deleted:
                            if (HandleFileDeleted(e.FullPath))
                            {
                                ReaddRoot(master);
                                return;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            DelayedRescan(e.ChangeType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnChanged failed");
        }
    }

    private void OnRenamed(object source, RenamedEventArgs e)
    {
        try
        {
            var ext = string.Empty;
            if (!string.IsNullOrEmpty(e.FullPath))
            {
                ext = Path.GetExtension(e.FullPath);
                ext = string.IsNullOrEmpty(ext) ? string.Empty : ext.Substring(1);
            }
            var ext2 = string.Empty;
            if (!string.IsNullOrEmpty(e.OldFullPath))
            {
                ext2 = Path.GetExtension(e.OldFullPath);
                ext2 = string.IsNullOrEmpty(ext2) ? string.Empty : ext2.Substring(1);
            }
            if (!Filter.Filtered(ext) && !Filter.Filtered(ext2))
            {
                Logger.LogDebug(
                  "Skipping name {name} {ext} {ext2}",
                  e.Name, ext, ext2);
                return;
            }
            Logger.LogDebug(
              "File System changed (rename, {changetype}): {fullpath} from {oldfullpath}", e.FullPath, e.OldFullPath, e.ChangeType);
            if (ids != null)
            {
                lock (_lock)
                {
                    var master = ids.GetItemById(Identifiers.GENERAL_ROOT) as VirtualFolder;
                    if (master != null)
                    {
                        var old = new FileInfo(e.OldFullPath);
                        // XXX prefix
                        if (directories.Contains(old.Directory))
                        {
                            if (HandleFileDeleted(e.OldFullPath) && HandleFileAdded(e.FullPath))
                            {
                                ReaddRoot(master);
                                return;
                            }
                        }
                        else
                        {
                            if (HandleFileAdded(e.FullPath))
                            {
                                ReaddRoot(master);
                                return;
                            }
                        }
                    }
                }
            }
            DelayedRescan(e.ChangeType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnRenamed failed");
        }
    }

    private void ReaddRoot(VirtualFolder master)
    {
        RegisterNewMaster(master);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void RegisterNewMaster(IMediaFolder newMaster)
    {
        lock (_lock)
        {
            ids.RegisterFolder(Identifiers.GENERAL_ROOT, newMaster);
            ids.RegisterFolder(
              Identifiers.SAMSUNG_IMAGES,
              new VirtualClonedFolder(
                newMaster,
                Identifiers.SAMSUNG_IMAGES,
                types & DlnaMediaTypes.Image
                )
              );
            ids.RegisterFolder(
              Identifiers.SAMSUNG_AUDIO,
              new VirtualClonedFolder(
                newMaster,
                Identifiers.SAMSUNG_AUDIO,
                types & DlnaMediaTypes.Audio
                )
              );
            ids.RegisterFolder(
              Identifiers.SAMSUNG_VIDEO,
              new VirtualClonedFolder(
                newMaster,
                Identifiers.SAMSUNG_VIDEO,
                types & DlnaMediaTypes.Video
                )
              );
        }
    }

    private void RescanInternal()
    {
        lock (_lockObject)
        {
            if (!rescanning)
            {
                Logger.LogDebug("Rescanning disabled");
                return;
            }

            if (isRescanning)
            {
                Logger.LogDebug("Already rescanning");
            }
            isRescanning = true;
        }
        Task.Factory.StartNew(() =>
        {
            try
            {
                Changing?.Invoke(this, EventArgs.Empty);

                try
                {
                    Logger.LogInformation("Rescanning {FriendlyName}...", FriendlyName);
                    DoRoot();
                    Logger.LogInformation("Done rescanning {FriendlyName}...", FriendlyName);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Rescan failed");
                }
                Changed?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                lock (_lockObject)
                {
                    isRescanning = false;
                }
            }
        }, TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
    }

    private void RescanTimer(object? sender, ElapsedEventArgs e)
    {
        RescanInternal();
    }

    internal bool Allowed(IMediaResource item)
    {
        lock (_lock)
        {
            return ids.Allowed(item);
        }
    }

    internal void DelayedRescan(WatcherChangeTypes changeType)
    {
        if (changeTimer.Enabled)
        {
            return;
        }
        switch (changeType)
        {
            case WatcherChangeTypes.Deleted:
                changeTimer.Interval = changeDeleteTime;
                break;

            case WatcherChangeTypes.Renamed:
                changeTimer.Interval = changeRenamedTime;
                break;

            default:
                changeTimer.Interval = changeDefaultTime;
                break;
        }
        var diff = DateTime.Now - lastChanged;
        if (diff.TotalSeconds <= 30)
        {
            changeTimer.Interval = Math.Max(
              TimeSpan.FromSeconds(20).TotalMilliseconds,
              changeTimer.Interval
              );
            Logger.LogInformation("Avoid thrashing {interval}", changeTimer.Interval);
        }
        Logger.LogDebug(
          "Change in {interval} on {friendlyname}",
          changeTimer.Interval,
          FriendlyName
          );
        changeTimer.Enabled = true;
        lastChanged = DateTime.Now;
    }

    internal BaseFile GetFile(PlainFolder aParent, FileInfo info)
    {
        BaseFile? item;
        lock (_lock)
        {
            item = ids.GetItemByPath(info.FullName) as BaseFile;
        }
        if (item != null &&
            item.InfoDate == info.LastWriteTimeUtc &&
            item.InfoSize == info.Length)
        {
            return item;
        }

        var ext = regSanitizeExt.Replace(
          info.Extension.ToUpperInvariant().Substring(1),
          string.Empty
          );
        var type = DlnaMaps.Ext2Dlna[ext];
        var mediaType = DlnaMaps.Ext2Media[ext];

        var rv = BaseFile.GetFile(aParent, info, type, mediaType, LoggerFactory);
        pendingFiles.Enqueue(new WeakReference(rv));
        return rv;
    }

    public void Load()
    {
        if (types == DlnaMediaTypes.Audio)
        {
            lock (_lock)
            {
                if (!ids.HasViews)
                {
                    ids.AddView("music");
                }
            }
        }
        DoRoot();

        changeTimer.AutoReset = false;
        changeTimer.Elapsed += RescanTimer;

        foreach (var watcher in watchers)
        {
            watcher.IncludeSubdirectories = true;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true;
        }

        watchTimer.Elapsed += RescanTimer;
        watchTimer.Enabled = true;
    }
}
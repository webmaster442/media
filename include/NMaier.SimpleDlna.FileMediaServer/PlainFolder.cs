using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.FileMediaServer.Files;
using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Interfaces.Metadata;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.FileMediaServer;

internal class PlainFolder : VirtualFolder, IMetaInfo
{
    private readonly DirectoryInfo dir;

    internal PlainFolder(FileServer server, VirtualFolder? parent, DirectoryInfo dir)
      : base(parent, dir.Name)
    {
        Server = server;
        this.dir = dir;
        var rawfiles = from f in dir.GetFiles("*.*")
                       select f;
        var files = new List<BaseFile>();
        foreach (var f in rawfiles)
        {
            var ext = f.Extension;
            if (string.IsNullOrEmpty(ext) ||
                !server.Filter.Filtered(ext.Substring(1)))
            {
                continue;
            }
            try
            {
                var file = server.GetFile(this, f);
                if (server.Allowed(file))
                {
                    files.Add(file);
                }
            }
            catch (Exception ex)
            {
                server.Logger.LogWarning(ex, f.FullName);
            }
        }
        Resources.AddRange(files);

        Folders = (from d in dir.GetDirectories()
                   let m = TryGetFolder(server, d)
                   where m != null && m.ChildCount > 0
                   select m as IMediaFolder).ToList();
    }

    public override string Path => dir.FullName;

    public FileServer Server { get; protected set; }

    public override string Title => dir.Name;

    public DateTime InfoDate => dir.LastWriteTimeUtc;

    public long? InfoSize => null;

    private PlainFolder? TryGetFolder(FileServer server, DirectoryInfo d)
    {
        try
        {
            return new PlainFolder(server, this, d);
        }
        catch (Exception ex)
        {
            if (!d.Name.Equals("System Volume Information"))
            {
                server.Logger.LogWarning(ex, "Failed to access folder");
            }
            return null;
        }
    }
}
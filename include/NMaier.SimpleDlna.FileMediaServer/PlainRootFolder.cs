using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.FileMediaServer;

internal sealed class PlainRootFolder : PlainFolder
{
    internal PlainRootFolder(FileServer server, DirectoryInfo di)
      : base(server, null, di)
    {
        Id = Identifiers.GENERAL_ROOT;
    }
}

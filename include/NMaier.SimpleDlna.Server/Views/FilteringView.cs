using System.Linq;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal abstract class FilteringView : BaseView, IFilteredView
{
    protected FilteringView(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    public abstract bool Allowed(IMediaResource item);

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
        oldRoot = new VirtualClonedFolder(oldRoot);
        ProcessFolder(oldRoot);
        return oldRoot;
    }

    private void ProcessFolder(IMediaFolder root)
    {
        foreach (var f in root.ChildFolders)
        {
            ProcessFolder(f);
        }
        foreach (var f in root.ChildItems.ToList())
        {
            if (Allowed(f))
            {
                continue;
            }
            root.RemoveResource(f);
        }
    }
}

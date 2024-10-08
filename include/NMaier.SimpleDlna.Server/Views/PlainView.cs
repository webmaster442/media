﻿using System;
using System.Linq;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal sealed class PlainView : BaseView
{
    public PlainView(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    public override string Description => "Mushes all files together into the root folder";

    public override string Name => "plain";

    private static void EatAll(IMediaFolder root, IMediaFolder folder)
    {
        foreach (var f in folder.ChildFolders.ToList())
        {
            EatAll(root, f);
        }
        foreach (var c in folder.ChildItems.ToList())
        {
            root.AddResource(c);
        }
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
        if (oldRoot == null)
        {
            throw new ArgumentNullException(nameof(oldRoot));
        }
        var rv = new VirtualFolder(null, oldRoot.Title, oldRoot.Id);
        EatAll(rv, oldRoot);
        return rv;
    }
}

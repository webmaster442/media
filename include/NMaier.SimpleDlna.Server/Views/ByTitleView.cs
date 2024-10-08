﻿using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using NMaier.SimpleDlna.Server.Interfaces;
using NMaier.SimpleDlna.Server.Types;

namespace NMaier.SimpleDlna.Server.Views;

internal sealed class ByTitleView : BaseView
{
    public ByTitleView(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    public override string Description => "Reorganizes files into folders by title";

    public override string Name => "bytitle";

    private static string GetTitle(IMediaResource res)
    {
        var pre = res.ToComparableTitle();
        if (string.IsNullOrEmpty(pre))
        {
            return "Unnamed";
        }
        return pre;
    }

    private void PartitionChildren(VirtualFolder folder, Prefixer prefixer, int startfrom = 1)
    {
        for (var wordcount = startfrom; ;)
        {
            var curwc = wordcount;
            var groups = from i in folder.ChildItems.ToList()
                         let prefix = prefixer.GetWordPrefix(GetTitle(i), curwc)
                         where !string.IsNullOrWhiteSpace(prefix)
                         group i by prefix.ToLowerInvariant()
                         into g
                         let gcount = g.LongCount()
                         where gcount > 3
                         orderby g.LongCount() descending
                         select g;
            var longest = groups.FirstOrDefault();
            if (longest == null)
            {
                if (wordcount++ > 5)
                {
                    return;
                }
                continue;
            }
            var newfolder = new VirtualFolder(folder, longest.Key);
            foreach (var item in longest)
            {
                folder.RemoveResource(item);
                newfolder.AddResource(item);
            }
            if (newfolder.ChildCount > 100)
            {
                PartitionChildren(newfolder, prefixer, wordcount + 1);
            }
            if (newfolder.ChildFolders.LongCount() == 1)
            {
                foreach (var f in newfolder.ChildFolders.ToList())
                {
                    folder.AdoptFolder(f);
                }
            }
            else
            {
                folder.AdoptFolder(newfolder);
            }
        }
    }

    private static void SortFolder(VirtualFolder folder, SimpleKeyedVirtualFolder titles)
    {
        foreach (var f in folder.ChildFolders.ToList())
        {
            var childFolder = f as VirtualFolder ?? throw new InvalidOperationException("Can't cast to VirtualFolder");
            SortFolder(childFolder, titles);
        }

        foreach (var c in folder.ChildItems.ToList())
        {
            var pre = GetTitle(c);
            pre = pre[0].ToString().ToUpperInvariant();
            titles.GetFolder(pre).AddResource(c);
            folder.RemoveResource(c);
        }
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
        var root = new VirtualClonedFolder(oldRoot);
        var titles = new SimpleKeyedVirtualFolder(root, "titles");
        SortFolder(root, titles);
        foreach (var i in root.ChildFolders.ToList())
        {
            root.ReleaseFolder(i);
        }
        foreach (var i in titles.ChildFolders.ToList())
        {
            if (i.ChildCount > 100)
            {
                Logger.LogDebug("Partioning folder {title}", i.Title);
                using (var prefixer = new Prefixer())
                {
                    var virtualFolder = i as VirtualFolder ?? throw new InvalidOperationException("Can't cast to Virtual Folder");
                    PartitionChildren(virtualFolder, prefixer);
                }
            }
            root.AdoptFolder(i);
        }
        return root;
    }

    private sealed class Prefixer : IDisposable
    {
        private static readonly Regex wordsplit = new Regex(@"(\b[^\s]+\b)", RegexOptions.Compiled);
        private static readonly Regex numbers = new Regex(@"[\d+._()\[\]+-]+", RegexOptions.Compiled);

        private readonly Dictionary<string, string[]> cache = new Dictionary<string, string[]>();

        public void Dispose()
        {
            cache.Clear();
        }

        public string? GetWordPrefix(string str, int wordcount)
        {
            string[]? m;
            var key = str.ToUpperInvariant();
            if (!cache.TryGetValue(key, out m))
            {
                m = (from w in wordsplit.Matches(str).Cast<Match>()
                     let v = numbers.Replace(w.Value, "").Trim()
                     where !string.IsNullOrWhiteSpace(v)
                     select v).ToArray();
                cache[key] = m;
            }
            if (m.Length < wordcount)
            {
                return null;
            }
            return string.Join(" ", m.Take(wordcount).ToArray());
        }
    }
}

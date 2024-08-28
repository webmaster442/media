namespace NMaier.SimpleDlna.FileMediaServer;

internal sealed class ExtensionFilter
{
    private static readonly StringComparer Cmp = StringComparer.OrdinalIgnoreCase;

    private readonly Dictionary<string, object?> exts = new Dictionary<string, object?>(Cmp);

    public ExtensionFilter(IEnumerable<string> extensions)
    {
        foreach (var e in extensions)
        {
            exts.Add(e, null);
        }
    }

    public bool Filtered(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            return false;
        }
        return exts.ContainsKey(extension);
    }
}
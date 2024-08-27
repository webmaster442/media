using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

using NMaier.SimpleDlna.Server.Interfaces;

namespace NMaier.SimpleDlna.Server.Types;

public partial class Headers : IHeaders
{
    [GeneratedRegex(@"^[a-z\d][a-z\d_.-]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "hu-HU")]
    private static partial Regex Validator();

    private readonly bool _asIs;

    private readonly Dictionary<string, string> _dict =
      new Dictionary<string, string>();

    protected Headers(bool asIs)
    {
        this._asIs = asIs;
    }

    public Headers()
      : this(false)
    {
    }

    public int Count => _dict.Count;

    public string HeaderBlock
    {
        get
        {
            var hb = new StringBuilder();
            foreach (var h in this)
            {
                hb.AppendFormat("{0}: {1}\r\n", h.Key, h.Value);
            }
            return hb.ToString();
        }
    }

    public Stream HeaderStream => new MemoryStream(Encoding.ASCII.GetBytes(HeaderBlock));

    public bool IsReadOnly => false;

    public ICollection<string> Keys => _dict.Keys;

    public ICollection<string> Values => _dict.Values;

    public string this[string key]
    {
        get { return _dict[Normalize(key)]; }
        set { _dict[Normalize(key)] = value; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    public void Add(KeyValuePair<string, string> item)
    {
        Add(item.Key, item.Value);
    }

    public void Add(string key, string value)
    {
        _dict.Add(Normalize(key), value);
    }

    public void Clear()
    {
        _dict.Clear();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        var p = new KeyValuePair<string, string>(
          Normalize(item.Key), item.Value);
        return _dict.Contains(p);
    }

    public bool ContainsKey(string key)
    {
        return _dict.ContainsKey(Normalize(key));
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _dict.Remove(Normalize(key));
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        return _dict.TryGetValue(Normalize(key), out value);
    }

    private string Normalize(string header)
    {
        if (!_asIs)
        {
            header = header.ToUpperInvariant();
        }
        header = header.Trim();
        if (!Validator().IsMatch(header))
        {
            throw new ArgumentException("Invalid header: " + header);
        }
        return header;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", from x in _dict select $"{x.Key}={x.Value}")})";
    }
}
namespace Media.Dto.Dlna;

public class SSDP
{
    private readonly Dictionary<string, string> _values;

    public SSDP()
    {
        _values = [];
    }

    public bool TryAdd(string key, string value)
    {
        return _values.TryAdd(key, value);
    }

    public string this[string key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    public string ST
    {
        get => _values["ST"];
        set => _values["ST"] = value;
    }

    public string Location
    {
        get => _values["LOCATION"];
        set => _values["LOCATION"] = value;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (var pair in _values)
        {
            sb.AppendLine($"{pair.Key.ToUpper()}: {pair.Value}");
        }
        return sb.ToString();
    }
}

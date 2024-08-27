using System.ComponentModel;

namespace NMaier.SimpleDlna.Server.Utilities;

public class ConfigParameters : AttributeCollection
{
    public ConfigParameters()
    {
    }

    public ConfigParameters(string parameters)
    {
        foreach (var valuesplit in parameters.Split(',').Select(p => p.Split(['='], 2)))
        {
            Add(valuesplit[0], valuesplit.Length == 2 ? valuesplit[1] : null);
        }
    }

    public bool TryGet<TValue>(string key, out TValue rv) where TValue : struct
    {
        return TryGet(key, out rv, StringComparer.CurrentCultureIgnoreCase);
    }

    public bool TryGet<TValue>(string key, out TValue rv, StringComparer comparer) where TValue : struct
    {
        rv = new TValue();
        if (rv is not IConvertible convertible)
        {
            throw new NotSupportedException("Not convertible");
        }
        switch (convertible.GetTypeCode())
        {
            case TypeCode.Boolean:
                foreach (var val in GetValuesForKey(key, comparer))
                {
                    try
                    {
                        rv = (TValue)(object)Formatting.Booley(val);
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                break;

            case TypeCode.Object:
                throw new NotSupportedException("Non pod types are not supported");
            default:
                var conv = TypeDescriptor.GetConverter(typeof(TValue));
                foreach (var val in GetValuesForKey(key, comparer))
                {
                    try
                    {
                        var converted = conv.ConvertFromString(val);
                        if (converted == null)
                        {
                            continue;
                        }
                        rv = (TValue)converted;
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                        // ignored
                    }
                }
                break;
        }
        return false;
    }

    public TValue Get<TValue>(string key, TValue defaultValue) where TValue : struct
    {
        return Get(key, defaultValue, StringComparer.CurrentCultureIgnoreCase);
    }

    public TValue Get<TValue>(string key, TValue defaultValue, StringComparer comparer)
      where TValue : struct
    {
        return TryGet(key, out TValue rv, comparer) ? rv : defaultValue;
    }

    public TValue? MaybeGet<TValue>(string key) where TValue : struct
    {
        return MaybeGet<TValue>(key, StringComparer.CurrentCultureIgnoreCase);
    }

    public TValue? MaybeGet<TValue>(string key, StringComparer comparer) where TValue : struct
    {
        if (TryGet(key, out TValue attempt, comparer))
        {
            return attempt;
        }
        return null;
    }
}
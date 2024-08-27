using System.Xml;

namespace NMaier.SimpleDlna.Server.Utilities;

public static class MoreDom
{
    public static XmlElement EL(this XmlDocument doc, string name)
    {
        return doc.EL(name, null, null);
    }

    public static XmlElement EL(this XmlDocument doc, string name, AttributeCollection? attributes)
    {
        return doc.EL(name, attributes, null);
    }

    public static XmlElement EL(this XmlDocument doc, string name, string? text)
    {
        return doc.EL(name, null, text);
    }

    public static XmlElement EL(this XmlDocument doc, string name, AttributeCollection? attributes, string? text)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }
        var rv = doc.CreateElement(name);
        if (text != null)
        {
            rv.InnerText = text;
        }
        if (attributes != null)
        {
            foreach (var i in attributes)
            {
                rv.SetAttribute(i.Key, i.Value);
            }
        }
        return rv;
    }
}
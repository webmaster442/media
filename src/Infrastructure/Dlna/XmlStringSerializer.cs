// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Media.Infrastructure.Dlna;

internal sealed class XmlStringSerializer<T> where T : class
{
    private readonly XmlSerializer _serializer;

    public XmlStringSerializer()
    {
        _serializer = new XmlSerializer(typeof(T));
    }

    public T? DeserializeSoap(string xml, bool decode)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
        nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

        XmlNode? bodyNode = xmlDoc.SelectSingleNode("//soap:Body", nsmgr);

        if (bodyNode != null)
        {
            string inner = bodyNode.InnerXml;
            if (decode)
            {
                inner = HttpUtility.HtmlDecode(bodyNode.InnerXml);
            }
            return Deserialize(inner);
        }

        return null;
    }

    public string SerializeSoap(T obj, bool encode)
    {
        string payload = Serialize(obj);
        if (encode)
            payload = HttpUtility.HtmlEncode(Serialize(obj));

        return $"""
            <?xml version=\"1.0\"?>
            <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/" s:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
                <s:Body>
                    {payload}
                </s:Body>
            </s:Envelope>
            """;
    }

    public T? Deserialize(string xml)
    {
        using var reader = new StringReader(xml);
        return _serializer.Deserialize(reader) as T;
    }

    public string Serialize(T obj)
    {
        using var writer = new StringWriter();
        _serializer.Serialize(writer, obj);
        return writer.ToString();
    }
}

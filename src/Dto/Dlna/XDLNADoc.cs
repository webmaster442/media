using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "X_DLNADOC", Namespace = "urn:schemas-dlna-org:device-1-0")]
public class XDLNADoc
{
    [XmlAttribute(AttributeName = "dlna", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string? Dlna { get; set; }
    [XmlText]
    public string? Text { get; set; }
}
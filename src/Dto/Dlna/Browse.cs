using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
[XmlRoot(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsNullable = false)]
public class Browse
{
    [XmlElement(Namespace = "")]
    public required string ObjectID { get; set; }

    [XmlElement(Namespace = "")]
    public required string BrowseFlag { get; set; }

    [XmlElement(Namespace = "")]
    public required string Filter { get; set; }

    [XmlElement(Namespace = "")]
    public required int StartingIndex { get; set; }

    [XmlElement(Namespace = "")]
    public required int RequestedCount { get; set; }

    [XmlElement(Namespace = "")]
    public required string SortCriteria { get; set; }
}

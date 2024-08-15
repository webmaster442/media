using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
[XmlRoot(ElementName = "albumArtURI", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/", IsNullable = false)]
public class AlbumArtURI
{
    [XmlAttribute(AttributeName = "profileID", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "urn:schemas-dlna-org:metadata-1-0/")]
    public string? ProfileID { get; set; }

    [XmlText]
    public string? Value { get;  set; }
}

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "iconList", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class IconList
{
    [XmlElement(ElementName = "icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public Icon[] Icon { get; set; }

    public IconList()
    {
        Icon = [];
    }
}

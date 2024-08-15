using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "serviceList", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class ServiceList
{
    [XmlElement(ElementName = "service", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public Service[] Service { get; set; }

    public ServiceList()
    {
        Service = [];
    }
}

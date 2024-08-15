// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "root", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class Root
{
    public Root()
    {
        SpecVersion = new SpecVersion();
        Device = new Device();
    }

    [XmlElement(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public SpecVersion SpecVersion { get; set; }
    [XmlElement(ElementName = "device", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public Device Device { get; set; }
    [XmlAttribute(AttributeName = "xmlns")]
    public string? Xmlns { get; set; }
}

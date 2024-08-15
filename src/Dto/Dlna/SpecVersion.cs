// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "specVersion", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class SpecVersion
{
    [XmlElement(ElementName = "major", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Major { get; set; }
    [XmlElement(ElementName = "minor", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Minor { get; set; }
}

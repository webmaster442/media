﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlRoot(ElementName = "icon", Namespace = "urn:schemas-upnp-org:device-1-0")]
public class Icon
{
    [XmlElement(ElementName = "mimetype", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Mimetype { get; set; }
    [XmlElement(ElementName = "width", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Width { get; set; }
    [XmlElement(ElementName = "height", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Height { get; set; }
    [XmlElement(ElementName = "depth", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Depth { get; set; }
    [XmlElement(ElementName = "url", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public string? Url { get; set; }
}

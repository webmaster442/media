// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class EnvelopeBody
{
    [XmlElement(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
    public BrowseResponse BrowseResponse { get; set; }

    public EnvelopeBody()
    {
        BrowseResponse = new BrowseResponse();
    }
}
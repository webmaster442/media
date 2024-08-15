// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
[XmlRoot(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsNullable = false)]
public class BrowseResponse
{
    public BrowseResponse()
    {
        Result = new Result();
    }

    [XmlElement(Namespace = "")]
    public Result Result { get; set; }

    [XmlElement(Namespace = "")]
    public int NumberReturned { get; set; }

    [XmlElement(Namespace = "")]
    public int TotalMatches { get; set; }

    [XmlElement(Namespace = "")]
    public int UpdateID { get; set; }
}

// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1")]
[XmlRoot(Namespace = "urn:schemas-upnp-org:service:ContentDirectory:1", IsNullable = false)]
public class BrowseRequest
{
    [XmlElement]
    public required string ObjectID { get; set; }

    [XmlElement]
    public required string BrowseFlag { get; set; }

    [XmlElement]
    public required string Filter { get; set; }

    [XmlElement]
    public required int StartingIndex { get; set; }

    [XmlElement]
    public required int RequestedCount { get; set; }

    [XmlElement]
    public required string SortCriteria { get; set; }
}

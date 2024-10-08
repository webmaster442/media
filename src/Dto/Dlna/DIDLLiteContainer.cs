﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
public class DIDLLiteContainer
{
    public DIDLLiteContainer()
    {
        Title = string.Empty;
        Class = string.Empty;
        Id = string.Empty;
        ParentID = string.Empty;
    }

    [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
    public string Title { get; set; }

    [XmlElement(ElementName = "class", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
    public string Class { get; set; }

    [XmlElement(ElementName = "storageUsed", Namespace = "urn:schemas-upnp-org:metadata-1-0/upnp/")]
    public long StorageUsed { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public string Id { get; set; }

    [XmlAttribute(AttributeName = "parentID")]
    public string ParentID { get; set; }

    [XmlAttribute(AttributeName = "restricted")]
    public byte Restricted { get; set; }

    [XmlAttribute(AttributeName = "searchable")]
    public byte Searchable { get; set; }

    [XmlAttribute(AttributeName = "childCount")]
    public int ChildCount { get; set; }
}

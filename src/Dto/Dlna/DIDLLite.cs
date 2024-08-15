// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/")]
[XmlRoot("DIDL-Lite", Namespace = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/", IsNullable = false)]
public class DIDLLite
{
    public DIDLLite()
    {
        Items = Array.Empty<object>();
    }

    [XmlElement("container", typeof(DIDLLiteContainer))]
    [XmlElement("item", typeof(DIDLLiteItem))]
    public object[] Items { get; set; }
}

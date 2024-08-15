// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Dlna;

[XmlType(AnonymousType = true, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
[XmlRoot(Namespace = "http://schemas.xmlsoap.org/soap/envelope/", IsNullable = false)]
public class Envelope
{
    public Envelope()
    {
        Body = new EnvelopeBody();
    }

    public EnvelopeBody Body { get; set; }

    [XmlAttribute(AttributeName = "encodingStyle", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string? EncodingStyle { get; set; }
}

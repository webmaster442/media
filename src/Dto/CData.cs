// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Media.Dto;

public class CData : IXmlSerializable
{
    private string _value;

    public CData() : this(string.Empty)
    {
    }

    public CData(string value)
    {
        _value = value;
    }

    public static implicit operator CData(string value) 
        => new CData(value);

    public static implicit operator string(CData cdata)
        => cdata._value;

    public XmlSchema? GetSchema() 
        => null;

    public void ReadXml(XmlReader reader) 
        => _value = reader.ReadElementString();

    public void WriteXml(XmlWriter writer) 
        => writer.WriteCData(_value);
}


using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommandParametersOption(
    string Description,
    [property: XmlAttribute()] string Short,
    [property: XmlAttribute()] string Long,
    [property: XmlAttribute()] string Value,
    [property: XmlAttribute()] bool Required,
    [property: XmlAttribute()] string Kind,
    [property: XmlAttribute()] string ClrType);

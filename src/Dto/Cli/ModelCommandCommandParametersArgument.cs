
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommandParametersArgument(
    string Description,
    [property: XmlAttribute()] string Name,
    [property: XmlAttribute()] int Position,
    [property: XmlAttribute()] bool Required,
    [property: XmlAttribute()] string Kind,
    [property: XmlAttribute()] string ClrType);

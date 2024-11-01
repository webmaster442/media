
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandParametersArgument(
    string Description,
    [property: XmlAttribute()] string Name,
    [property: XmlAttribute()] byte Position,
    [property: XmlAttribute()] bool Required,
    [property: XmlAttribute()] string Kind,
    [property: XmlAttribute()] string ClrType);

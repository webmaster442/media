
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommand(
    string Description,
    ModelCommandCommandParameters Parameters,
    [property: XmlAttribute()] string Name,
    [property: XmlAttribute()] bool IsBranch,
    [property: XmlAttribute()] string ClrType,
    [property: XmlAttribute()] string Settings);

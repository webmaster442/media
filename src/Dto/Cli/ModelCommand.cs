
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommand(
    string Description,
    ModelCommandParameters Parameters,
    [property: XmlElement("Command")] ModelCommandCommand[] Command,
    [property: XmlAttribute()] string Name,
    [property: XmlAttribute()] bool IsBranch,
    [property: XmlAttribute()] bool IsDefault,
    [property: XmlIgnore()] bool IsDefaultSpecified,
    [property: XmlAttribute()] string ClrType,
    [property: XmlAttribute()] string Settings);

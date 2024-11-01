
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandParameters(
    [property: XmlElement("Argument")] ModelCommandParametersArgument[] Argument,
    [property: XmlElement("Option")] ModelCommandParametersOption[] Option);


using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommandParameters(
    [property: XmlElement("Argument")] ModelCommandCommandParametersArgument[] Argument,
    [property: XmlElement("Option")] ModelCommandCommandParametersOption[] Option);

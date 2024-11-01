
using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public record Model([property: XmlElement("Command")] ModelCommand[] Command);

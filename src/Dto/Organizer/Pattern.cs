using System.Xml.Serialization;

using Media.Infrastructure.Validation;

namespace Media.Dto.Organizer;

[XmlRoot("Pattern")]
public class Pattern
{
    [XmlAttribute]
    [NotEmptyOrWiteSpace]
    public required string Value { get; set; }

    [XmlAttribute]
    public bool IsRegex { get; set; }

    [XmlAttribute]
    public bool IgnoreCase { get; set; }
}
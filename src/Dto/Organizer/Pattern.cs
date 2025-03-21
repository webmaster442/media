// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

using Media.Infrastructure.Validation;

namespace Media.Dto.Organizer;

[XmlRoot("Pattern")]
public record class Pattern
{
    [XmlAttribute]
    [NotEmptyOrWiteSpace]
    public required string Value { get; set; }

    [XmlAttribute]
    public bool IsRegex { get; set; }

    [XmlAttribute]
    public bool IgnoreCase { get; set; }
}
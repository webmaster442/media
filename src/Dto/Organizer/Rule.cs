// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

using Media.Infrastructure.Validation;

namespace Media.Dto.Organizer;

public class Rule
{
    [XmlElement("Pattern")]
    public required Pattern[] Patterns { get; set; }

    [XmlAttribute]
    [NotEmptyOrWiteSpace]
    public required string Folder { get; set; }

    override public string ToString() => Folder;
}

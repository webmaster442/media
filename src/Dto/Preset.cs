// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

using Media.Infrastructure.Validation;

namespace Media.Dto;

public class Preset
{
    [XmlAttribute("Name")]
    [NotEmptyOrWiteSpace]
    public required string Name { get; set; }

    [XmlElement("CommandLine")]
    [NotEmptyOrWiteSpace]
    [IsValidCommandLine]
    public required string CommandLine { get; set; }

    [XmlAttribute("Extension")]
    [NotEmptyOrWiteSpace]
    public required string Extension { get; set; }

    [XmlAttribute("Category")]
    [NotEmptyOrWiteSpace]
    public required string Category { get; set; }

    [XmlElement("Description")]
    [NotEmptyOrWiteSpace]
    public required string Description { get; set; }

    [XmlIgnore]
    public const string InputPlaceHolder = "{input}";

    [XmlIgnore]
    public const string OutputPlaceHolder = "{output}";
}

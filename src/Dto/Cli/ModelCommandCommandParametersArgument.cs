// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------


using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommandParametersArgument(
    string Description,
    [property: XmlAttribute()] string Name,
    [property: XmlAttribute()] int Position,
    [property: XmlAttribute()] bool Required,
    [property: XmlAttribute()] string Kind,
    [property: XmlAttribute()] string ClrType);

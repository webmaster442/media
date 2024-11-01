// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------


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

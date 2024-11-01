// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------


using System.Xml.Serialization;

namespace Media.Dto.Cli;

[XmlType(AnonymousType = true)]
public record ModelCommandCommandParameters(
    [property: XmlElement("Argument")] ModelCommandCommandParametersArgument[] Argument,
    [property: XmlElement("Option")] ModelCommandCommandParametersOption[] Option);

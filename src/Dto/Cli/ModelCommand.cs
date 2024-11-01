// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Cli;

[Serializable]
[XmlType(AnonymousType = true)]
public class ModelCommand
{
    [XmlElement]
    public string Description { get; set; }

    [XmlElement("Command")]
    public ModelCommand[] Commands { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public bool IsBranch { get; set; }

    [XmlAttribute]
    public bool IsDefault { get; set; }

    public ModelCommand()
    {
        Description = string.Empty;
        Commands = Array.Empty<ModelCommand>();
        Name = string.Empty;
    }
}

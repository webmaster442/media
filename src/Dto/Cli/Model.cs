// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Xml.Serialization;

namespace Media.Dto.Cli;

[Serializable]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public class Model
{
    [XmlElement("Command")]
    public ModelCommand[] Commands { get; set; }

    public Model()
    {
        Commands = Array.Empty<ModelCommand>();
    }
}

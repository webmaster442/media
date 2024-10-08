﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure.Validation;

namespace Media.Infrastructure;

internal class BaseBachSettings : ValidatedCommandSettings
{
    [Description("Bach project file")]
    [CommandOption("-p|--project")]
    [Required]
    [FileExists]
    public string ProjectName { get; set; }

    public BaseBachSettings()
    {
        var files = Directory.GetFiles(Environment.CurrentDirectory, "*.bach");
        ProjectName = files.Length == 1 ? files[0] : string.Empty;
    }
}

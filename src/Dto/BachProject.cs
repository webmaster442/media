// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto;

internal class BachProject
{
    public string OutputDirectory { get; set; } = string.Empty;
    public List<string> Args { get; set; } = [];
    public string ConversionCommand { get; set; } = string.Empty;
    public List<string> Files { get; set; } = [];
}

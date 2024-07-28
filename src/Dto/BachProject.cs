namespace FFCmd.Dto;

internal class BachProject
{
    public string OutputDirectory { get; set; } = string.Empty;
    public string PresetAndArguments { get; set; } = string.Empty;
    public List<string> Files { get; set; } = [];
}

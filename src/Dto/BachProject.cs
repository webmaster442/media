namespace FFCmd.Dto;

internal class BachProject
{
    public string OutputDirectory { get; set; } = string.Empty;
    public List<string> Args { get; set; } = [];
    public string ConversionCommand { get; set; } = string.Empty;
    public List<string> Files { get; set; } = [];
}

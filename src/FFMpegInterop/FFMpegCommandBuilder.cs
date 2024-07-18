namespace FFCmd.FFMpegInterop;

internal sealed class FFMpegCommandBuilder
{
    private enum CliSegment
    {
        InputFile = int.MinValue,
        OutputFile = int.MaxValue
    }

    private readonly Dictionary<CliSegment, string> _data;

    private readonly Dictionary<CliSegment, string> _segmentFormats = new()
    {
        { CliSegment.InputFile, "-i \"{0}\"" },
        { CliSegment.OutputFile, "\"{0}\"" }
    };

    private void SetArgument(CliSegment segment, string value)
    {
        _data[segment] = string.Format(_segmentFormats[segment], value);
    }

    public FFMpegCommandBuilder()
    {
        _data = new Dictionary<CliSegment, string>();
    }

    public FFMpegCommandBuilder WithInputFile(string inputFile)
    {
        SetArgument(CliSegment.InputFile, inputFile);
        return this;
    }

    public FFMpegCommandBuilder WithOutputFile(string outputFile)
    {
        SetArgument(CliSegment.OutputFile, outputFile);
        return this;
    }

    public string BuildCommandLine()
    {
        var ordered = _data.OrderBy(x => x.Key).Select(x => x.Value);
        return string.Join(" ", ordered);
    }
}

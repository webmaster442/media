using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal class Info : AsyncCommand<Info.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Input file")]
        [CommandArgument(0, "<input>")]
        [FileExists]
        [Required]
        public string InputFile { get; init; }

        public Settings()
        {
            InputFile = string.Empty;
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var result = await FFProbe.GetFFProbeResult(settings.InputFile);

            var relevantInformations = FFProbe.Transform(result);

            Terminal.DisplayObject(relevantInformations);

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
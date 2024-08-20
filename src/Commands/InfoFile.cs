// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal class InfoFile : AsyncCommand<InfoFile.Settings>
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
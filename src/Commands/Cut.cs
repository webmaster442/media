// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal sealed class Cut : Command<Cut.Settings>
{
    public sealed class Settings : ValidatedCommandSettings
    {
        [Description("Input file")]
        [CommandArgument(0, "<input>")]
        [FileExists]
        public string Input { get; init; } = string.Empty;

        [Description("Output file")]
        [CommandArgument(1, "<output>")]
        [SameExtensionAs(nameof(Input))]
        [Required]
        public string OutputFile { get; init; } = string.Empty;

        [Description("Start time")]
        [CommandOption("-s|--start")]
        [Required]
        public string StartTime { get; init; } = string.Empty;

        [Description("End time")]
        [CommandOption("-e|--end")]
        [Required]
        public string EndTime { get; init; } = string.Empty;
    }

    private static bool TryParseAsSeconds(string value, out double seconds)
    {
        if (TimeSpan.TryParse(value, out var timeSpan))
        {
            seconds = timeSpan.TotalSeconds;
            return true;
        }
        else if (double.TryParse(value, CultureInfo.InvariantCulture, out seconds))
        {
            return true;
        }
        seconds = 0;
        return false;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!TryParseAsSeconds(settings.StartTime, out var startTime))
        {
            Terminal.RedText("Invalid start time");
            return ExitCodes.Error;
        }

        if (!TryParseAsSeconds(settings.EndTime, out var endTime))
        {
            Terminal.RedText("Invalid end time");
            return 1;
        }

        if (startTime >= endTime)
        {
            Terminal.RedText("Start time must be less than end time");
            return 1;
        }

        try
        {
            FFMpegCommandBuilder builder = new();
            builder.WithInputFile(settings.Input)
                .WithOutputFile(settings.OutputFile)
                .WithAudioCodec("copy")
                .WithVideoCodec("copy")
                .WithStartTimeInSeconds(startTime)
                .WithDurationInSeconds(endTime - startTime);

            var cmdLine = builder.Build();
            Terminal.InfoText("Generated arguments:");
            Terminal.InfoText(cmdLine);

            FFMpeg.Start(cmdLine);
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }
}
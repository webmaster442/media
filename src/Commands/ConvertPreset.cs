// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

[Example("Convert a media file using a preset", "media convert preset presetname input.mp4 output.mkv")]
internal sealed class ConvertPreset : AsyncCommand<ConvertPreset.Settings>
{
    private readonly FFMpeg _ffmpeg;

    public class Settings : ValidatedCommandSettings
    {
        [Required]
        [CommandArgument(0, "<preset>")]
        [Description("Preset name")]
        public string PresetName { get; set; }

        [Required]
        [FileExists]
        [Description("Input file")]
        [CommandArgument(1, "<input>")]
        public string InputFile { get; set; }

        [Required]
        [NotEmptyOrWiteSpace]
        [Description("Output file")]
        [CommandArgument(2, "<output>")]
        public string OutputFile { get; set; }

        public Settings()
        {
            InputFile = string.Empty;
            OutputFile = string.Empty;
            PresetName = string.Empty;
        }
    }

    public ConvertPreset(ConfigAccessor configAccessor)
    {
        _ffmpeg = new FFMpeg(configAccessor);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        Dictionary<string, Preset> presets = await Presets.LoadPresetsAsync();

        if (!presets.TryGetValue(settings.PresetName, out Preset? preset))
        {
            Terminal.RedText($"Preset {settings.PresetName} not found");
            return ExitCodes.Error;
        }

        var outputExtension = Path.GetExtension(settings.OutputFile);
        if (!string.Equals(outputExtension, preset.Extension, StringComparison.OrdinalIgnoreCase))
        {
            Terminal.RedText($"Output file extension must be {preset.Extension}");
            return ExitCodes.Error;
        }

        var cmdline = preset.GetCommandLine(settings.InputFile, settings.OutputFile);

        _ffmpeg.Start(cmdline);

        return ExitCodes.Success;
    }
}

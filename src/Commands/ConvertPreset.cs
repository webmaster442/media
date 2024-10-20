// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto;
using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interfaces;
using Media.Interop;

namespace Media.Commands;

internal sealed class ConvertPreset : AsyncCommand<ConvertPreset.Settings>
{
    private readonly FFMpeg _ffmpeg;
    private readonly IDryRunResultAcceptor _dryRunResultAcceptor;

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

    public ConvertPreset(ConfigAccessor configAccessor, IDryRunResultAcceptor acceptor)
    {
        _ffmpeg = new FFMpeg(configAccessor);
        _dryRunResultAcceptor = acceptor;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
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

            if (_dryRunResultAcceptor.Enabled)
            {
                _dryRunResultAcceptor.Result = cmdline;
            }
            else
            {
                _ffmpeg.Start(cmdline);
            }

            return ExitCodes.Success;

        }
        catch (Exception ex)
        {
            Terminal.DisplayException(ex);
            return ExitCodes.Exception;
        }
    }
}

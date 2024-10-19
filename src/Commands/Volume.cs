// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Infrastructure;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;
internal class Volume : Command<Volume.Arguments>
{
    internal class Arguments : ValidatedCommandSettings
    {
        [Range(0, 100)]
        [Description("Set the system audio volume")]
        [CommandArgument(0, "[volume]")]
        public int? Volume { get; set; }

        [Description("Mute/Unmut the system audio")]
        [CommandOption("-m|--mute")]
        public bool Mute { get; set; }
    }

    public override int Execute(CommandContext context, Arguments settings)
    {
        AudioController.Interact(systemAudio =>
        {
            if (settings.Mute)
            {
                if (systemAudio.IsSystemMuted)
                {
                    systemAudio.Mute(false);
                    Terminal.InfoText("Speakers UnMuted 🔊");
                }
                else
                {
                    systemAudio.Mute(true);
                    Terminal.InfoText("Speakers Muted 🔇");
                }
            }
            else if (settings.Volume.HasValue)
            {
                float value = settings.Volume.Value / 100f;
                systemAudio.SetVolume(value);
                Terminal.InfoText($"Volume set to {settings.Volume}%");
            }
            else
            {
                Terminal.InfoText($"Volume is {systemAudio.GetVolume() * 100}%");
            }
        });

        return ExitCodes.Success;
    }
}

using Media.Infrastructure;

namespace Media.Commands.Presets;

internal sealed class CreateShellScript : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        Terminal.InfoText("Creating convert.cmd...");
        var loaded = await Media.Infrastructure.PresetProvider.LoadPresetsAsync();

        StringBuilder switchLogic = new();
        StringBuilder labels = new();

        var presets = loaded.Values.OrderBy(x => x.Name);

        switchLogic.AppendLine("@echo off");
        foreach (var preset in presets)
        {
            var s = $"if \"%1\"==\"{preset.Name}\" goto {preset.Name}";
            switchLogic.AppendLine(s);

            labels.AppendLine($":{preset.Name}");
            labels.AppendLine($"echo encoding with preset {preset.Name}...");
            labels.Append("ffmpeg");
            labels.AppendLine($" {preset.GetCommandLine("%2", "%3")}");
            labels.AppendLine("goto exit");
            labels.AppendLine();
        }

        switchLogic.AppendLine("goto usage");

        labels.AppendLine(":usage");
        labels.AppendLine("echo usage:");
        labels.AppendLine("echo convert ^<preset^> ^<input^> ^<output^>");
        labels.AppendLine("echo.");
        labels.AppendLine("echo presets:");
        foreach (var preset in presets)
        {
            labels.AppendLine($"echo {preset.Name}");
        }
        labels.AppendLine("goto exit");
        labels.AppendLine();
        labels.AppendLine(":exit");

        await using var script = File.CreateText("convert.cmd");
        await script.WriteLineAsync(switchLogic.ToString());
        await script.WriteLineAsync(labels.ToString());
        
        Terminal.InfoText("Done");

        return ExitCodes.Success;
    }
}

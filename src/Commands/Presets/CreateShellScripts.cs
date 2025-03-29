
using Media.Dto;
using Media.Infrastructure;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Media.Commands.Presets;

internal sealed class CreateShellScript : AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {

        var loaded = await Media.Infrastructure.PresetProvider.LoadPresetsAsync();
        var presets = loaded.Values.OrderBy(x => x.Name);

        await CreateCmd(presets);
        await CreatePs1(presets);

        return ExitCodes.Success;
    }

    private async Task CreatePs1(IOrderedEnumerable<Preset> presets)
    {
        Terminal.InfoText("Creating convert.ps1...");
        StringBuilder ps = new StringBuilder();
        ps.AppendLine("param(")
          .AppendLine("    [string]$preset,")
          .AppendLine("    [string]$input,")
          .AppendLine("    [string]$output")
          .AppendLine(")")
          .AppendLine();

        ps.AppendLine("if (-not $preset -or -not $input -or -not $output) {")
          .AppendLine("    usage()")
          .AppendLine("}");

        ps.AppendLine();
        ps.AppendLine("switch ($preset) {");

        foreach (var preset in presets)
        {
            ps.AppendLine($"    \"{preset.Name}\" {{ {preset.Name}() }}");
        }
        ps.AppendLine("    default { usage() }");
        ps.AppendLine("}");
        ps.AppendLine();

        foreach (var preset in presets)
        {
            ps.AppendLine($"function {preset.Name}() {{");
            ps.AppendLine($"    Write-Host \"encoding with preset {preset.Name}...\"");
            ps.Append("    ffmpeg");
            ps.AppendLine($" {preset.GetCommandLine("$input", "$output")}");
            ps.AppendLine("}");
            ps.AppendLine();
        }

        ps.AppendLine("function usage() {")
          .AppendLine("    Write-Host \"usage:\"")
          .AppendLine("    Write-Host \"convert <preset> <input> <output>\"")
          .AppendLine("    Write-Host \"\"")
          .AppendLine("    Write-Host \"presets:\"");

        foreach (var preset in presets)
        {
            ps.AppendLine($"    Write-Host \"{preset.Name}\"");
        }
        ps.AppendLine("}");
        await using var script = File.CreateText("convert.ps1");
        await script.WriteLineAsync(ps.ToString());
        Terminal.InfoText("Done");
    }

    private static async Task CreateCmd(IOrderedEnumerable<Dto.Preset> presets)
    {
        Terminal.InfoText("Creating convert.cmd...");
        StringBuilder switchLogic = new();
        StringBuilder labels = new();

        switchLogic.AppendLine("@echo off");
        foreach (var preset in presets)
        {
            var s = $"if \"%1\"==\"{preset.Name}\" goto {preset.Name}";
            switchLogic.AppendLine(s);

            labels.AppendLine($":{preset.Name}")
                  .AppendLine($"echo encoding with preset {preset.Name}...")
                  .Append("ffmpeg")
                  .AppendLine($" {preset.GetCommandLine("%2", "%3")}")
                  .AppendLine("goto exit")
                  .AppendLine();
        }

        switchLogic.AppendLine("goto usage");

        labels.AppendLine(":usage")
              .AppendLine("echo usage:")
              .AppendLine("echo convert ^<preset^> ^<input^> ^<output^>")
              .AppendLine("echo.")
              .AppendLine("echo presets:");

        foreach (var preset in presets)
        {
            labels.AppendLine($"echo {preset.Name}");
        }

        labels.AppendLine("goto exit")
              .AppendLine()
              .AppendLine(":exit");

        await using var script = File.CreateText("convert.cmd");
        await script.WriteLineAsync(switchLogic.ToString());
        await script.WriteLineAsync(labels.ToString());

        Terminal.InfoText("Done");
    }
}

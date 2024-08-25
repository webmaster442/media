// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Embedded;
using Media.Infrastructure;
using Media.Interop;

using Spectre.Console;

namespace Media.Commands;

internal sealed class InfoHwEncoders : AsyncCommand
{
    private readonly FFMpeg _ffMpeg;

    public InfoHwEncoders(ConfigAccessor configAccessor)
    {
        _ffMpeg = new FFMpeg(configAccessor);
    }

    private async Task<bool> IsSupported(string encoderName)
    {
        var file = Path.Combine(AppContext.BaseDirectory, EmbeddedResources.TestImage);

        var cmd = $"-hide_banner -i \"{file}\" -c:v {encoderName} -t 10 -f null -";

        using var process = _ffMpeg.CreateProcess(cmd,
                                                  redirectStdIn: false,
                                                  redirectStdOut: false,
                                                  redirectStderr: true);

        process.Start();
        var result = await process.StandardError.ReadToEndAsync();

        return !result.Contains("Conversion failed!");
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            await EmbeddedResources.ExtractAsync(EmbeddedResources.TestImage);

            var encoders = _ffMpeg
                .GetEncoders()
                .Where(e => e.Type == Dto.Internals.FFMpegEncoderInfo.EncoderType.Video
                       && (e.Name.EndsWith("_amf") || e.Name.EndsWith("_nvenc") || e.Name.EndsWith("_qsv")))
                .OrderBy(x => x.Name);

            var table = new Table();
            table.AddColumns("Name", "Type", "Description", "Hardware availabe");

            foreach (var encoder in encoders)
            {
                Terminal.InfoText($"Testing {encoder.Name}...");
                var supported = await IsSupported(encoder.Name);
                var icon = supported
                    ? $"{Emoji.Known.CheckMark} yes"
                    : $"{Emoji.Known.CrossMark} no";
                table.AddRow(encoder.Name, encoder.Type.ToString(), encoder.Description, icon);
            }

            AnsiConsole.Write(table);

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }

    }
}
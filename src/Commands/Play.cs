// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.Selector;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal sealed class Play : AsyncCommand<Play.Settings>
{
    private readonly Mpv _mpv;

    public class Settings : ValidatedCommandSettings
    {
        [Description("Input file")]
        [CommandArgument(0, "[input]")]
        [FileExists(IsOptional = true)]
        public string InputFile { get; init; }

        [Description("Use DLNA for browsing")]
        [CommandOption("-d|--dlna")]
        public bool Dlna { get; init; }

        public Settings()
        {
            InputFile = string.Empty;
        }

        public override Spectre.Console.ValidationResult Validate()
        {
            if (Dlna && !string.IsNullOrEmpty(InputFile))
            {
                return Spectre.Console.ValidationResult.Error("DLNA and file input cannot be used together");
            }
            return base.Validate();
        }
    }

    public Play(ConfigAccessor configAccessor)
    {
        _mpv = new Mpv(configAccessor);
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(settings.InputFile))
            {
                string file = string.Empty;

                file = settings.Dlna
                    ? await DoDlnaBrowse()
                    : await DoFileSelection();

                await RunMpv(file);

                return ExitCodes.Success;
            }

            await RunMpv(settings.InputFile);

            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
    }

    private async Task RunMpv(string fileName)
    {
        string cmd = $"\"{fileName}\" --input-ipc-server=\\\\.\\pipe\\mpvsocket";
        using var process = _mpv.CreateProcess(cmd,
                                               redirectStdIn: false,
                                               redirectStdOut: false,
                                               redirectStderr: false);

        process.Start();

        var webapp = new MpvWebControllerApp(process.Id, "mpvsocket");

        await webapp.RunAsync(CancellationToken.None);
        Terminal.InfoText("Press a key to exit...");
        Console.ReadKey();
    }

    private static async Task<string> DoDlnaBrowse()
    {
        using var consoleCancel = new ConsoleCancelTokenSource();
        using var provider = new DlnaItemProvider();

        var selector = new ItemSelector<DlnaItem, DlnaItemProvider.CurrentPath>(
            itemProvider: provider,
            title: "Select a media server",
            defaultPath: DlnaItemProvider.CurrentPath.Empty);

        var selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

        return selectedItem.Uri.ToString();
    }

    private static async Task<string> DoFileSelection()
    {
        using var consoleCancel = new ConsoleCancelTokenSource();

        var selector = new ItemSelector<Item, string>(
            itemProvider: new FileSystemItemProvider(Mpv.GetSupportedExtensions()),
            title: "Select a file",
            defaultPath: string.Empty);

        Item selectedItem = await selector.SelectItemAsync(consoleCancel.Token);

        return $"\"{selectedItem.FullPath}\"";
    }
}

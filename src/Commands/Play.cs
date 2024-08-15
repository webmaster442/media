// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.Internals;
using Media.Infrastructure;
using Media.Infrastructure.Selector;
using Media.Infrastructure.Validation;
using Media.Interop;

namespace Media.Commands;

internal class Play : AsyncCommand<Play.Settings>
{
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

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            Mpv.EnsureIsInstalled();

            if (string.IsNullOrWhiteSpace(settings.InputFile))
            {
                string file = string.Empty;

                if (settings.Dlna)
                    file = await DoDlnaBrowse();
                else
                    file = await DoFileSelection();

                Mpv.Start(file);
                return ExitCodes.Success;
            }

            Mpv.Start(settings.InputFile);
            return ExitCodes.Success;
        }
        catch (Exception e)
        {
            Terminal.DisplayException(e);
            return ExitCodes.Exception;
        }
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

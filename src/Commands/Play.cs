// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;

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

        public Settings()
        {
            InputFile = string.Empty;
        }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            Mpv.EnsureIsInstalled();

            if (string.IsNullOrWhiteSpace(settings.InputFile))
            {
                var file = await DoFileSelection();
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

    private static async Task<string> DoFileSelection()
    {
        using var consoleCancel = new ConsoleCancelTokenSource();
        var selector = new ItemSelector<Item>(new FileSystemItemProvider(Mpv.GetSupportedExtensions()), "Select a file");
        var selectedItem = await selector.SelectItemAsync(consoleCancel.Token);
        return $"\"{selectedItem.FullPath}\"";
    }
}

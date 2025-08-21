
using Spectre.Console;
using Spectre.Console.Cli;

using Webmaster442.WindowsTerminal;

var app = new CommandApp();
app.Configure(cfg =>
{
    cfg.AddDelegate<ProgressSettings>("progress", OnProgesss);
    cfg.AddDelegate("hideprogress", OnHideProgress);
});

int OnHideProgress(CommandContext context)
{
    Terminal.SetProgressbar(ProgressbarState.Hidden, 0);
    return 0;
}

int OnProgesss(CommandContext context, ProgressSettings settings)
{
    int percent = (int)Math.Ceiling(((double)settings.Current / settings.Total) * 100.0d);
    Terminal.SetProgressbar(ProgressbarState.Default, percent);
    return 0;
}
internal sealed class ProgressSettings : CommandSettings
{
    [CommandArgument(0, "<Total>")]
    public int Total { get; set; }
    [CommandArgument(1, "<Current>")]
    public int Current { get; set; }

    public override ValidationResult Validate()
    {
        if (Total < 0)
            return ValidationResult.Error("Total can't be negative");

        if (Current < 0)
            return ValidationResult.Error("Current can't be negative");

        if (Current > Total)
            return ValidationResult.Error("Current must be less than total");

        return ValidationResult.Success();
    }

}

using System.Reflection;

using Spectre.Console;

namespace Media.Commands;

internal class DefaultCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Media.Notes.txt");
        if (stream == null)
            throw new InvalidOperationException("Can't find notes");

        using var reader = new StreamReader(stream);

        var content = reader.ReadToEnd();

        AnsiConsole.WriteLine(content);

        return ExitCodes.Success;
    }
}

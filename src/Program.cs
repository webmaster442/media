using System.Text.RegularExpressions;

using Spectre.Console.Cli;

static string ConvertToKebabCase(string input)
{
    if (string.IsNullOrEmpty(input))
    {
        return string.Empty;
    }
    return KebabCaseRegex().Replace(input, "-$1").ToLower();
}

static void AddCommands(IConfigurator config)
{
    var commands = typeof(Program).Assembly.GetTypes()
        .Where(t => t.IsAssignableTo(typeof(ICommand))
               && t.IsClass
               && !t.IsAbstract)
        .ToList();

    var add = config.GetType().GetMethod(nameof(config.AddCommand));

    if (add == null)
        throw new InvalidOperationException();

    foreach (var command in commands)
    {
        var name = ConvertToKebabCase(command.Name) ?? throw new InvalidOperationException();
        add.MakeGenericMethod(command).Invoke(config, [name]);
    }
}

var app = new CommandApp();
app.Configure(config => AddCommands(config));

app.Run(args);

partial class Program
{
    [GeneratedRegex("(?<!^)([A-Z])")]
    private static partial Regex KebabCaseRegex();
}
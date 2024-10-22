// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Reflection;

using Spectre.Console;

namespace Media.Infrastructure;

internal sealed class ExampleGenerator
{
    private readonly ExampleAttribute[] _examples;

    public ExampleGenerator()
    {
        var commandType = typeof(ICommand);

        var commands = typeof(ExampleGenerator).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(commandType));

        _examples = commands
            .OrderBy(t => t.Name)
            .SelectMany(x => x.GetCustomAttributes<ExampleAttribute>()).ToArray();
    }

    public IEnumerable<string> GenerateExamples()
    {
        static string GenerateExampleString(ExampleAttribute example)
        {
            return $"""
                [bold]{example.Description.EscapeMarkup()}[/]:

                    [italic green]{example.Example.EscapeMarkup()}[/]

                """;
        }

        foreach (var example in _examples)
        {
            yield return GenerateExampleString(example);
        }
    }
}

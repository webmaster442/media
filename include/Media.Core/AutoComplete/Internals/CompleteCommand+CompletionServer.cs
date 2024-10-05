// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Text.Json;

using Media.Core.AutoComplete.Internals;

using Spectre.Console.Rendering;

namespace JKToolKit.Spectre.AutoCompletion.Completion.Internals;

public partial class CompleteCommand
{
    private async Task<int> RunCompletionServer(CompleteCommandSettings settings)
    {
        while (true)
        {
            var line = await Console.In.ReadLineAsync();
            if (
                line is null
                || string.IsNullOrWhiteSpace(line)
                || string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            try
            {
                var args = GetLineParams(line);
                var parser = new CommandCompletionContextParser(_model, _configuration);
                var ctx = parser.Parse(
                    args.Command,
                    args.CursorPosition);

                var completions = await GetCompletionsAsync(ctx);
                RenderCompletion(completions, settings);
            }
            catch (Exception e)
            {
                // ignored
                Console.WriteLine(e.ToString().Replace("\n", "\\n"));
                return -1;
            }
        }
    }

    private static TabCompletionArgs GetLineParams(string line)
    {
        var result = new TabCompletionArgs(line);

        // When starts with { and ends with }, it's a json object
        var normalizedLine = line.Trim(' ', '\t', '\r', '\n');
        var couldBeJson = normalizedLine.StartsWith('{') && normalizedLine.EndsWith('}');
        if (couldBeJson)
        {
            try
            {
                var deserialized = JsonSerializer.Deserialize<TabCompletionArgs>(line, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });
                if (deserialized != null)
                {
                    result = deserialized;
                }
            }
            catch
            {
                // ignored
            }
        }

        return result;
    }

    private class JsonSingleLineRenderable<T> : IRenderable
    {
        private readonly T _value;

        public JsonSingleLineRenderable(T value)
        {
            _value = value;
        }

        public Measurement Measure(RenderOptions options, int maxWidth)
        {
            return new Measurement(maxWidth, maxWidth);
        }

        public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
        {
            return new Segment[] { new Segment(JsonSerializer.Serialize(_value)) };
        }
    }

    private static class JsonSingleLineRenderable
    {
        public static JsonSingleLineRenderable<T> Create<T>(T value)
        {
            return new JsonSingleLineRenderable<T>(value);
        }
    }
}
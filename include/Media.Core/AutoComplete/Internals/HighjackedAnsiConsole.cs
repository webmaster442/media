// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Media.Core.AutoComplete.Internals;

internal class HighjackedAnsiConsole : IAnsiConsole
{
    public IAnsiConsole OriginalConsole { get; }

    public HighjackedAnsiConsole(IAnsiConsole console)
    {
        OriginalConsole = console;
    }

    public Profile Profile => OriginalConsole.Profile;

    public IAnsiConsoleCursor Cursor => OriginalConsole.Cursor;
    public IAnsiConsoleInput Input => OriginalConsole.Input;
    public IExclusivityMode ExclusivityMode => OriginalConsole.ExclusivityMode;
    public RenderPipeline Pipeline => OriginalConsole.Pipeline;

    public void Clear(bool home)
    {
    }

    public void Write(IRenderable renderable)
    {
    }
}
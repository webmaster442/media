// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
internal sealed class ExampleAttribute : Attribute
{
    public string Description { get; }
    public string Example { get; }

    public ExampleAttribute(string description, string example)
    {
        Description = description;
        Example = example;
    }
}

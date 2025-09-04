// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.CommandAttributes;

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

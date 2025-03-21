// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.CommandAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class InstallerDataAttribute: Attribute
{
    public required string Arguments { get; init; }
    public required int IconIndex { get; init; }
    public required string Name { get; init; }
}
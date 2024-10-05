// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Core.AutoComplete;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public sealed class CompletionSuggestionsAttribute : Attribute
{
    public string[] Suggestions { get; }

    public CompletionSuggestionsAttribute(params string[] suggestions)
    {
        Suggestions = suggestions;
    }
}
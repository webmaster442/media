using JKToolKit.Spectre.AutoCompletion.Completion;

using Media.Core.AutoComplete;

namespace Media.Tests.Autocomplete.Settings;

// mycommand user add [name] --age [age]
internal class UserAddSettings : CommandSettings
{
    [CommandArgument(0, "<name>")]
    [System.ComponentModel.Description("The name of the user.")]
    [CompletionSuggestions("Angelika", "Arnold", "Bernd", "Cloud", "Jonas")]
    public string Name { get; set; } = string.Empty;

    [CommandOption("-a|--age <age>")]
    [System.ComponentModel.Description("The age of the user.")]
    [CompletionSuggestions("10", "15", "20", "30")]
    public int Age { get; set; }

    [CommandOption("-g|--gender <gender>")]
    public string Gender { get; set; } = string.Empty;
}
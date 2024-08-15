// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Spectre.Console;

namespace Media.Infrastructure.Selector;

internal class ItemSelector<T> where T : IITem
{
    private readonly IItemProvider<T> _itemProvider;
    private string _currentPath;

    public string Title { get; set; }

    public ItemSelector(IItemProvider<T> itemProvider, string title)
    {
        _itemProvider = itemProvider;
        Title = title;
        _currentPath = string.Empty;
    }

    private string Convert(T item)
        => _itemProvider.ConvertItem(item).EscapeMarkup();

    public async Task<T> SelectItemAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        while (true)
        {
            var items = await _itemProvider.GetItemsAsync(_currentPath, cancellationToken);

            var selection = new SelectionPrompt<T>()
                .Title($"{Title}\r\n[green]{_currentPath}[/]")
                .PageSize(Console.WindowHeight - 5)
                .AddChoices(items)
                .UseConverter(Convert);

            var item = await selection.ShowAsync(AnsiConsole.Console, cancellationToken);

            if (_itemProvider.SelectionCanExit(item))
            {
                return item;
            }
            else
            {
                _currentPath = _itemProvider.ModifyCurrentPath(item);
            }
        }
    }
}
// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Threading;

using Spectre.Console;

namespace Media.Infrastructure.Selector;

internal class ItemSelector<TItem, TPath> where TItem : IITem
{
    private readonly IItemProvider<TItem, TPath> _itemProvider;
    private TPath _currentPath;

    public string Title { get; set; }

    public ItemSelector(IItemProvider<TItem, TPath> itemProvider, string title, TPath defaultPath)
    {
        _itemProvider = itemProvider;
        Title = title;
        _currentPath = defaultPath;
    }

    private string Convert(TItem item)
        => _itemProvider.ConvertItem(item).EscapeMarkup();

    public async Task<TItem> SelectItemAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Clear();
        while (true)
        {
            var items = await _itemProvider.GetItemsAsync(_currentPath, cancellationToken);

            var selection = new SelectionPrompt<TItem>()
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
                _currentPath = _itemProvider.SelectCurrentPath(item);
            }
        }
    }
}
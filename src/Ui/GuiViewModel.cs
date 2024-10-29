using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Media.Dto.Internals;
using Media.Interfaces;
using Media.Ui.Controls;

namespace Media.Ui;

internal partial class GuiViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private GuiCommand? _selectedCommand;

    public ObservableRangeCollection<GuiCommand> Commands { get; }

    public GuiViewModel()
    {
        Commands = new ObservableRangeCollection<GuiCommand>();
    }

    public void Initialize()
    {
        Commands.AddRange(GuiCommands.Commands);
        SelectedCommand = Commands[0];
    }

    [RelayCommand]
    private void Launch()
    {

    }
}

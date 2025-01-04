// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Windows;

using Media.BaseCommands;
using Media.DbAdapters;
using Media.Infrastructure;
using Media.Interfaces;
using Media.Ui;

using Microsoft.Extensions.Logging;

namespace Media.Commands;
internal sealed class Gui : BaseGuiCommand<GuiWindow>
{
    private readonly RadioStationsClient _radioStationsClient;
    private readonly GuiDatabaseAdapter _guiDatabaseAdapter;
    private readonly ConfigAdapter _configAdapter;

    public Gui(
        RadioStationsClient radioStationsClient,
        GuiDatabaseAdapter guiDatabaseAdapter,
        ConfigAdapter configAdapter)
    {
        _radioStationsClient = radioStationsClient;
        _guiDatabaseAdapter = guiDatabaseAdapter;
        _configAdapter = configAdapter;
    }

    protected override IViewModel? CreateDataContext(IUiFunctions uiFunctions, ILoggerFactory loggerFactory)
    {
        return new GuiViewModel(uiFunctions,
                                _radioStationsClient,
                                _guiDatabaseAdapter,
                                _configAdapter,
                                loggerFactory);
    }

    protected override IWindowManipulator? CreateWindowManipulator()
        => new WindowManipulator();

    internal class WindowManipulator : IWindowManipulator
    {
        public Size GetWindowSize(Size xamlDefinedWindowSize, Size workArea)
        {
            if (workArea.Width > 1280 && workArea.Height > 720)
            {
                return new Size(1920, 1080);
            }
            return xamlDefinedWindowSize;
        }

        public Point GetWindowStartupLocation(Size workArea, Size windowSize)
        {
            return new Point
            {
                X = (workArea.Width - windowSize.Width) / 2,
                Y = (workArea.Height - windowSize.Height) / 2
            };
        }
    }
}

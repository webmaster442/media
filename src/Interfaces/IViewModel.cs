// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Media.Interfaces;

internal interface IViewModel : INotifyPropertyChanged
{
    void Initialize();
    ILogger Logger { get; }
}
// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Ui.Gui;

internal class DriveModel
{
    public required string Letter { get; init; }
    public required string Label { get; init; }
    public required DriveType DriveType { get; init; }
    public required double PecentFull { get; init; }
    public required string TotalSize { get; init; }
    public required string FreeSpace { get; init; }
}

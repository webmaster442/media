// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interfaces;

internal enum ProgressState
{
    None,
    Indeterminate,
    Normal,
    Error,
    Paused,
}
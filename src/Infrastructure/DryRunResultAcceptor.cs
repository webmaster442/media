// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Interfaces;

namespace Media.Infrastructure;

internal sealed class DryRunResultAcceptor : IDryRunResultAcceptor
{
    public bool Enabled { get; set; }

    public string Result { get; set; } = string.Empty;
}

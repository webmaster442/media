﻿// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Interfaces;

internal interface IDryRunResultAcceptor
{
    string Result { get; set; }

    bool Enabled { get; set; }
}

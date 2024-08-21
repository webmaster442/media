// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using System.Diagnostics;

var fileName = Environment.GetCommandLineArgs()[0];

var outputFile = Path.Combine(AppContext.BaseDirectory, Path.ChangeExtension(fileName, ".txt"));

File.WriteAllLines(outputFile, args);

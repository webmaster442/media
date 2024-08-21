using System.Diagnostics;

var fileName = Environment.GetCommandLineArgs()[0];

var outputFile = Path.Combine(AppContext.BaseDirectory, Path.ChangeExtension(fileName, ".txt"));

File.WriteAllLines(outputFile, args);

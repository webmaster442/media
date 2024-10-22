// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Dto;

internal sealed class CrashReport
{
    public required DateTime Time { get; init; }
    public required string WorkDirectory { get; init; }
    public required string[] StartArguments { get; init; }
    public required string OsVersion { get; init; }
    public required string ExceptionMessage { get; init; }
    public required string Source { get; init; }
    public required string[] StackTrace { get; init; }
}

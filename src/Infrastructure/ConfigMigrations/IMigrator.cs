// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

namespace Media.Infrastructure.ConfigMigrations;

public interface IMigrator
{
    Version Version { get; }
    void Migrate(IDictionary<string, string> keyValuePairs);
}

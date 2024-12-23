// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database;

namespace Media.DbAdapters;

internal class DatabaseAdapterBase
{
    protected DatabaseContext GetContext()
    {
        var db = new DatabaseContext();
        db.RunMigrationsIfNeeded();
        return db;
    }
}

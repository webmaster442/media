// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024-2025 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Media.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Metadata> Metadata { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<PlayedEntry> PlayedEntries { get; set; }
    public DbSet<ApiCacheEntry> ApiCacheEntries { get; set; }
    public DbSet<FolderBookmark> FolderBookmarks { get; set; }

    public DatabaseContext()
    {
        DbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "media.cli");
    }
    public void RunMigrationsIfNeeded()
    {
        var migrations = Database.GetPendingMigrations();
        if (migrations.Any())
        {
            Database.Migrate();
        }
    }

    public string DbFile { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbFile}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder.Entity<Setting>());
        Configure(modelBuilder.Entity<PlayedEntry>());
        Configure(modelBuilder.Entity<ApiCacheEntry>());
        Configure(modelBuilder.Entity<FolderBookmark>());
        Configure(modelBuilder.Entity<Metadata>());
    }

    private static void Configure(EntityTypeBuilder<FolderBookmark> builder)
    {
        builder.HasKey(x => x.Path);
        builder.Property(x => x.Name).IsRequired();
    }

    private static void Configure(EntityTypeBuilder<ApiCacheEntry> builder)
    {
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Value).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ValidityInSeconds).IsRequired();
    }

    private static void Configure(EntityTypeBuilder<PlayedEntry> builder)
    {
        builder.HasKey(x => x.Path);
        builder.Property(x => x.LastPlayed).IsRequired();
    }

    private static void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Value).IsRequired();
    }

    private static void Configure(EntityTypeBuilder<Metadata> builder)
    {
        builder.HasKey(x => x.Path);
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Artist).IsRequired();
        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.Size).IsRequired();
        builder.Property(x => x.PlayTimeInSeconds).IsRequired();
        builder.Property(x => x.DiscNumber).IsRequired();
        builder.Property(x => x.TrackNumber).IsRequired();
        builder.Property(x => x.Codecs).IsRequired();
        builder.Property(x => x.VideoWidth).IsRequired();
        builder.Property(x => x.VideoHeight).IsRequired();

        builder.HasIndex(x => x.Artist);
        builder.HasIndex(x => x.Album);
        builder.HasIndex(x => x.Genre);
        builder.HasIndex(x => x.Year);
        builder.HasIndex(x => x.Size);
    }
}

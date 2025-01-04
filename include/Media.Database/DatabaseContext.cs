// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------


using Media.Database.Entity;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Media.Database;

public class DatabaseContext : DbContext
{
    public DbSet<MusicFile> Musics { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<VideoFile> Videos { get; set; }
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
        Configure(modelBuilder.Entity<MusicFile>());
        Configure(modelBuilder.Entity<Album>());
        Configure(modelBuilder.Entity<Genre>());
        Configure(modelBuilder.Entity<Setting>());
        Configure(modelBuilder.Entity<VideoFile>());
        Configure(modelBuilder.Entity<PlayedEntry>());
        Configure(modelBuilder.Entity<ApiCacheEntry>());
        Configure(modelBuilder.Entity<FolderBookmark>());
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

    private static void Configure(EntityTypeBuilder<VideoFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Path).IsRequired();
        builder.Property(x => x.Size);
        builder.Property(x => x.AddedDate);
        builder.Property(x => x.PlayTimeInSeconds);
        builder.Property(x => x.Width);
        builder.Property(x => x.Height);
        builder.Property(x => x.Codecs);
        builder.HasIndex(x => x.Path).IsUnique();
    }

    private static void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(x => x.Key);
        builder.Property(x => x.Value).IsRequired();
    }

    private static void Configure(EntityTypeBuilder<MusicFile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Artist).IsRequired();
        builder.Property(x => x.AddedDate);
        builder.Property(x => x.Year);
        builder.Property(x => x.Path).IsRequired();
        builder.Property(x => x.Size);
        builder.Property(x => x.PlayTimeInSeconds);
        builder.Property(x => x.DiscNumber);
        builder.Property(x => x.TrackNumber);
        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.Artist);
        builder.HasIndex(x => x.Path).IsUnique();
        builder.HasIndex(x => x.Year);
    }
    private static void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Artist).IsRequired();
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Artist);
        builder.HasIndex(x => x.Name);
    }
    private static void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

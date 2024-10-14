// -----------------------------------------------------------------------------------------------
// Copyright (c) 2024 Ruzsinszki Gábor
// This code is licensed under MIT license (see LICENSE for details)
// -----------------------------------------------------------------------------------------------

using Media.Dto.MediaDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Media.Database;

internal class MediaDatabaseContext : DbContext
{
    public DbSet<MusicFile> Music { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<VideoFile> Video { get; set; }

    public MediaDatabaseContext(DbContextOptions<MediaDatabaseContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configure(modelBuilder.Entity<Album>());
        Configure(modelBuilder.Entity<Genre>());
        Configure(modelBuilder.Entity<MusicFile>());
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

using Media.Dto.MediaDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Media.Database;

internal record class AlbumData(string Name, uint Id);

internal sealed class MediaDbSerives : IDisposable
{
    private readonly MediaDatabaseContext _context;
    private readonly ILogger _logger;

    public MediaDbSerives(MediaDatabaseContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<List<string>> GetGenres()
    {
        return await _context.Genres
            .AsNoTracking()
            .Select(g => g.Name)
            .ToListAsync();
    }

    public async Task<List<AlbumData>> GetAlbums()
    {
        return await _context.Albums
            .AsNoTracking()
            .OrderBy(a => a.Year)
            .Select(a => new AlbumData($"{a.Artist} - {a.Name}", a.Id))
            .ToListAsync();
    }

    public async Task<List<string>> GetArtists()
    {
        return await _context.Music
            .AsNoTracking()
            .Select(x => x.Artist)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<uint>> GetYears()
    {
        return await _context.Music
            .AsNoTracking()
            .Select(x => x.Year)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<MusicFile>> GetMusicFilesByYear(uint year)
    {
        return await _context.Music
            .AsNoTracking()
            .Where(x => x.Year == year)
            .ToListAsync();
    }

    public async Task<List<MusicFile>> GetMusicFilesByArtist(string artist)
    {
        return await _context.Music
            .AsNoTracking()
            .Where(x => x.Artist == artist)
            .ToListAsync();
    }

    public async Task<List<MusicFile>> GetMusicFilesByGenre(string genre)
    {
        return await _context.Music
            .AsNoTracking()
            .Include(x => x.Genre)
            .Where(x => x.Genre!.Name == genre)
            .ToListAsync();
    }

    public async Task<List<MusicFile>> GetMusicFilesByAlbum(AlbumData album)
    {
        return await _context.Music
            .AsNoTracking()
            .Include(x => x.Album)
            .Where(x => x.Album!.Id == album.Id)
            .ToListAsync();
    }

    public async Task<int> AddFiles(IEnumerable<string> files)
    {
        return await EntityFactory.CreateEntities(_context, _logger, files);
    }
}

namespace Media.Dto.MediaDb;

public class Genre
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<MusicFile> Tracks { get; set; } = new HashSet<MusicFile>();
}

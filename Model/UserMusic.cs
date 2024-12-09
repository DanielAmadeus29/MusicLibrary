namespace MusicLibrary.Models;


public class UserMusic
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? PlaylistId { get; set; }
    public Playlist Playlist { get; set; }
}

namespace MusicLibrary.Models
{
    public class PlaylistSongs
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int UserMusicId { get; set; }

        // Navigation properties
        public Playlist Playlist { get; set; }
        public UserMusic UserMusic { get; set; }
    }
}

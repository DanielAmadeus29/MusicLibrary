namespace MusicLibrary.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserMusic> Songs { get; set; } // Optional, if a relationship is needed
    }
}

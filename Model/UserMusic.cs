namespace MusicLibrary.Models
{
    public class UserMusic
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key to the Users table
        public string Title { get; set; } // Song title
        public string Artist { get; set; } // Song artist
        public string FilePath { get; set; } // Path to the stored file
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp of creation
    }
}

namespace MusicLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 

 
        public ICollection<UserMusic> UserMusics { get; set; } = new List<UserMusic>();
    }
}

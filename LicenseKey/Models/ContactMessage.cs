using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SendDate { get; set; } = DateTime.Now;

        public ContactMessage(string user, string imageUrl, string role, string room, string message)
        {
            User = user;
            ImageUrl = imageUrl;
            Role = role;
            Room = room;
            Message = message;
        }
    }
}
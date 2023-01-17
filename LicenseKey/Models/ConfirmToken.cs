using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseKey.Models
{
    public class ConfirmToken
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Token { get; set; }
        public string ForgetVerify { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime ConfirmedAt { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public ConfirmToken(string token) 
        { 
            this.Token = token;
        }
    }
}

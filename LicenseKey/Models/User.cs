using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseKey.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Image { get; set; } = string.Empty;
        public string? ImagePublicIP { get; set; } = string.Empty;
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; } = "USER";
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Birthday { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsVerified { get; set; } = false;

        public virtual ConfirmToken? ConfirmToken { get; set; }
    }
}

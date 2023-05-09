using LicenseKey.Models;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Helpers.Dto
{
    public class UserDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? Image { get; set; } = string.Empty;
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Birthday { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsVerified { get; set; } = false;

    }
}

using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request
{
    public class CreateUserReq
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Birthday { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request
{
    public class UpdateUserInfoRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        public string? Name { get; set;}
        public string? PhotoUrl { get; set; }

    }
}

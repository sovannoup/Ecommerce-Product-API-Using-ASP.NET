using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request
{
    public class UpdateUserInfoRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        public string Name { get; set;} = string.Empty;
        public string? PhotoUrl { get; set; }

    }
}

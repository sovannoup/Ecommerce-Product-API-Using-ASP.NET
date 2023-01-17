using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request
{
    public class UploadFeedbackRequest
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}

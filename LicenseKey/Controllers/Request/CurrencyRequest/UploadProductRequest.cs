using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request.ProductRequest
{
    public class UploadProductRequest
    {
        [Required]
        public string Name { get; set; }
        public IFormFile? LogoUrl { get; set; }
        [Required]
        public int Total { get; set; }
        [Required]
        public List<string> LicenseKeyTo { get; set; }
    }
}

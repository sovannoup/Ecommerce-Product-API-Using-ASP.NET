using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LicenseKey.Helpers.Dto
{
    public class ProductDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [JsonPropertyName("Image")]
        [Required]
        public IFormFile? LogoUrl { get; set; }
        [Required]
        public int TotalUnit { get; set; }
        [Required]
        public int AvaiUnit { get; set; }
    }
}

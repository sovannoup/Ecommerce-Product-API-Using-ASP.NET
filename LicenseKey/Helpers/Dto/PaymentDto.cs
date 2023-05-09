using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LicenseKey.Helpers.Dto
{
    public class PaymentDto
    {
        public int? Id { get; set; }
        [JsonPropertyName("Image")]
        public IFormFile? PhotoUrl { get; set; }
        public string? Title { get; set; } = string.Empty;
        public float Fee { get; set; }
        public float AddOn { get; set; }
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
    }
}

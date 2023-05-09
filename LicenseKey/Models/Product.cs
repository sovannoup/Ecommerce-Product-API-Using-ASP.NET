using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public float Price { get; set; }
        [JsonPropertyName("Image")]
        public string LogoUrl { get; set; } = string.Empty;
        public string ImagePublicIP { get; set; } = string.Empty;
        public int TotalUnit { get; set; }
        public int AvaiUnit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}


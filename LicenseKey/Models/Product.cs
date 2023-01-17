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
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("Image")]
        [Required]
        public string? LogoUrl { get; set; }
        [Required]
        public string ImagePublicIP { get; set; }
        [Required]
        public int Total { get; set; }
        public UserTransaction? UserTransaction { get; set; }
        [Required]
        public List<string> LicenseKeyTo { get; set; }

        public Product(string name, string logoUrl, string imagePublicIP, int total, List<string> LicenseKeyTo)
        {
            Name = name;
            LogoUrl = logoUrl;
            ImagePublicIP = imagePublicIP;
            Total = total;
            this.LicenseKeyTo = LicenseKeyTo;
        }
    }
}

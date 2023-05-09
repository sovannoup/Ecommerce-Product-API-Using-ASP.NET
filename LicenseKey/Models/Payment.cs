using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseKey.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? PhotoUrl { get; set; } = string.Empty;
        public string ImagePublicIP { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public float Fee { get; set; }
        public float AddOn { get; set; }
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public DateTime UploadAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;

    }
}

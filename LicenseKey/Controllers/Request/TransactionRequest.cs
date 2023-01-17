using LicenseKey.Models;
using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Controllers.Request
{
    public class TransactionRequest
    {
        [Required]
        public float Amount { get; set; }
        [Required]
        public float ReceievedAmount { get; set; }
        public string? Message { get; set; }
        [Required]
        public int FromCurId { get; set; }
        [Required]
        public int ToCurId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LicenseKey.Models
{
    public class UserTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public float Amount { get; set; }
        public string Message { get; set; } = string.Empty;
        [Required]
        public float ReceievedAmount { get; set; }
        [Required]
        public int ProductFromId { get; set; }
        [Required]
        public int ProductToId { get; set; }
        public List<Product>? Product { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime SuccessDate { get; set; } = DateTime.Now;
        public bool? IsSuccess { get; set; } = false;
        public int? UserId { get; set; }
        public User? User { get; set; }

        public UserTransaction(float amount, float receievedAmount)
        {
            Amount = amount;
            ReceievedAmount = receievedAmount;
        }
    }
}

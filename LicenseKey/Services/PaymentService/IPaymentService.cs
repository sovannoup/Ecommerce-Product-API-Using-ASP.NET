using LicenseKey.Controllers.Request;
using LicenseKey.Helpers.Dto;
using LicenseKey.Models;

namespace LicenseKey.Services.PaymentService
{
    public interface IPaymentService
    {
        public Task<string> UploadPaymentMethod(PaymentDto payment);
        public List<Payment> GetPayments();
        public Task<string> UpdatePayment(PaymentDto payment);
        public string DeletePayment(int id);
    }
}

using LicenseKey.Helpers.Dto;
using LicenseKey.Models;
using LicenseKey.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public PaymentController(IConfiguration configuration, IPaymentService paymentService)
        {
            _configuration = configuration;
            _paymentService = paymentService;
        }

        [HttpGet]
        //[Authorize]
        public List<Payment> GetPayments()
        {
            return _paymentService.GetPayments();
        }

        [HttpPost]
        //[Authorize]
        public Task<string> UploadPayment([FromForm] PaymentDto payment)
        {
            return _paymentService.UploadPaymentMethod(payment);
        }

        [HttpPut]
        //[Authorize]
        public Task<string> UpdatePayment([FromForm] PaymentDto payment)
        {
            //string? auth = HttpContext.Request.Headers["Authorization"];
            return _paymentService.UpdatePayment(payment);
        }

        [HttpDelete("id")]
        //[Authorize]
        public string DeletePayment(int id)
        {
            //string? auth = HttpContext.Request.Headers["Authorization"];
            return _paymentService.DeletePayment(id);
        }

    }
}

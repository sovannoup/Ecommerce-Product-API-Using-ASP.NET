using LicenseKey.Controllers.Request;
using LicenseKey.Services.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITransactionService _transactionService;

        public TransactionController(IConfiguration configuration, ITransactionService transactionService)
        {
            _configuration = configuration;
            _transactionService = transactionService;
        }

        [HttpPost]
        [Authorize]
        public string Purchase(TransactionRequest req)
        {
            string? auth = HttpContext.Request.Headers["Authorization"];
            return _transactionService.Purchase(req, auth);
        }

        [HttpPut("id")]
        [Authorize]
        public string SetSuccessTransaction(int id)
        {
            string? auth = HttpContext.Request.Headers["Authorization"];
            return _transactionService.SetSuccessTransaction(id, auth);
        }

        [HttpPut("id")]
        [Authorize]
        public string SetFailureTransaction(int id)
        {
            string? auth = HttpContext.Request.Headers["Authorization"];
            return _transactionService.SetFailureTransaction(id, auth);
        }
    }
}

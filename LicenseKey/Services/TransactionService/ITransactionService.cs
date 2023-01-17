using LicenseKey.Controllers.Request;

namespace LicenseKey.Services.Transaction
{
    public interface ITransactionService
    {
        public string Purchase(TransactionRequest req, string auth);
        public string SetSuccessTransaction(int id, string auth);
        public string SetFailureTransaction(int id, string auth);
    }
}

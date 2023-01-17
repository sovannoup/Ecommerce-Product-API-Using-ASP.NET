using LicenseKey.Controllers.Request;
using LicenseKey.Models;
using LicenseKey.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LicenseKey.Services.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;

        public TransactionService(ApplicationDbContext applicationDbContext, IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
        }

        public string Purchase(TransactionRequest req, string auth)
        {
            string[] token = auth.Split(" ");
            string email = VerifyToken(token[1]);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email);

            Product? FromCur = _applicationDbContext.Product.FirstOrDefault(x => x.Id == req.FromCurId);
            Product? ToCur = _applicationDbContext.Product.FirstOrDefault(x => x.Id == req.ToCurId);
            if (FromCur == null || ToCur == null)
            {
                throw new Exception("Unknown Product");
            }

            UserTransaction userTran = new(req.Amount, req.ReceievedAmount)
            {
                ProductFromId = FromCur.Id,
                ProductToId = ToCur.Id,
                Message = req.Message ?? "",
                UserId = user.Id
            };
            _applicationDbContext.UserTransaction.Add(userTran);
            _applicationDbContext.SaveChanges();
            return "Success";
        }

        public string SetSuccessTransaction(int id, string auth)
        {
            string[] token = auth.Split(" ");
            string email = VerifyToken(token[1]);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email);
            UserTransaction tran = _applicationDbContext.UserTransaction.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (tran == null)
            {
                throw new Exception("Transaction not found");
            }
            tran.IsSuccess = true;
            tran.SuccessDate = DateTime.Now;
            _applicationDbContext.UserTransaction.Update(tran); 
            _applicationDbContext.SaveChanges();
            return "Success";
        }
        
        public string SetFailureTransaction(int id, string auth)
        {
            string[] token = auth.Split(" ");
            string email = VerifyToken(token[1]);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email);
            UserTransaction? tran = _applicationDbContext.UserTransaction.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (tran == null)
            {
                throw new Exception("Transaction not found");
            }
            tran.IsSuccess = false;
            _applicationDbContext.UserTransaction.Update(tran); 
            _applicationDbContext.SaveChanges();
            return "Success";
        }

        public string VerifyToken(string token)
        {
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            if (token == null)
                throw new Exception("Token required");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetConnectionString("JWT_Token"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string? Email = jwtToken.Claims?.FirstOrDefault(x => x.Type == "email").Value;
                return Email;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}

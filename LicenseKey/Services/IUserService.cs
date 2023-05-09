using LicenseKey.Controllers.Request;
using LicenseKey.Controllers.Response;
using LicenseKey.Helpers.Dto;
using LicenseKey.Models;

namespace LicenseKey.Services
{
    public interface IUserService
    {
        UserDto GetUser(string auth);
        ResponseObj Login(UserLoginRequest request);
        Task<string> CreateUser(CreateUserReq req);
        string UpdateUser(UpdateUserInfoRequest req);
        string DeleteUser(string auth);
        bool SendMail(string mail, string content);
        string ConfirmEmail(string token);
        string ForgetPassword(ForgetPasswordRequest req);
        string ChangePassword(string token, string password);
    }
}

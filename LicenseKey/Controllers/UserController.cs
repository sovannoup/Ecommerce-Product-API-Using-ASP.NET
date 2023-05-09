using LicenseKey.Controllers.Request;
using LicenseKey.Controllers.Response;
using LicenseKey.Helpers.Dto;
using LicenseKey.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ResponseObj Login(UserLoginRequest request)
        {
            return _userService.Login(request);
        }

        [Authorize]
        [HttpGet]
        public UserDto GetUser()
        {
            string? auth = HttpContext.Request.Headers["Authorization"];
            return _userService.GetUser(auth);
        }

        [AllowAnonymous]
        [HttpPost]
        public Task<string> CreateUser([FromForm] CreateUserReq user)
        {
           return _userService.CreateUser(user);
        }

        [Authorize]
        [HttpPost]
        public string UpdateUser(UpdateUserInfoRequest req)
        {
            return _userService.UpdateUser(req);
        }

        [Authorize]
        [HttpDelete]
        public string DeleteUser()
        {
            string? auth = HttpContext.Request.Headers["Authorization"];
            return _userService.DeleteUser(auth);
        }

        [AllowAnonymous]
        [HttpGet("token")]
        public string ConfirmEmail(string token)
        {
           return _userService.ConfirmEmail(token);
        }
        [AllowAnonymous]
        [HttpPost]
        public string ForgetPassword(ForgetPasswordRequest req)
        {
            return _userService.ForgetPassword(req);
        }
        [AllowAnonymous]
        [HttpPost]
        public string ChangePassword([FromHeader] string token, string password) 
        {
            return _userService.ChangePassword(token, password);
        }

    }
}

﻿using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using LicenseKey.Controllers.Request;
using LicenseKey.Controllers.Response;
using LicenseKey.Helpers;
using LicenseKey.Helpers.Dto;
using LicenseKey.Models;
using LicenseKey.Repository;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LicenseKey.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinaryDotNet;

        public UserService(ApplicationDbContext applicationDbContext, IConfiguration configuration, IMapper mapper, IOptions<CloudinarySettings> config)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _mapper = mapper;
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinaryDotNet = new Cloudinary(acc);
        }

        public ResponseObj Login(UserLoginRequest request)
        {
            //auth failed - creds incorrect
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new AppException("Username or password is incorrect");

            var token = CreateToken(user, 60);

            ResponseObj obj = new();
            obj.Result["token"] = token;
            obj.Result["user"] = user;


            return obj;
        }

        public string ChangePassword(string token, string password)
        {
            if (token == null)
                throw new Exception("Token is required");

            ConfirmToken? ct = _applicationDbContext.ConfirmToken.FirstOrDefault(x => x.ForgetVerify == token) ?? throw new Exception("Invalid link");
            string? email = VerifyToken(token);

            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email) ?? throw new Exception("Invalid token");
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var encrypted = BCrypt.Net.BCrypt.HashPassword(password, salt);

            user.Password = encrypted;
            _applicationDbContext.Users.Update(user);
            ct.ForgetVerify = string.Empty;
            _applicationDbContext.ConfirmToken.Update(ct);
            _applicationDbContext.SaveChanges();

            return "Success";
        }

        public string ConfirmEmail(string token)
        {
            if (token == null)
                return "Invalid confirm link";
            string email = VerifyToken(token);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email);
            if(user is null)
            {
                return "User not found";
            }
            ConfirmToken? ct = _applicationDbContext.ConfirmToken.FirstOrDefault(x => x.Token == token);
            if(ct == null)
            {
                return "Invalid token";
            }else if(ct.IsConfirmed == true)
            {
                return "Account already confirmed";
            }
            ct.IsConfirmed = true;
            _applicationDbContext.SaveChanges();

            return "Success";
        }

        public async Task<string> CreateUser(CreateUserReq req)
        {
            User? isExist = _applicationDbContext.Users.FirstOrDefault(x => x.Email == req.Email );
            if (isExist != null) {
                throw new Exception("User already exist");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var encrypted = BCrypt.Net.BCrypt.HashPassword(req.Password, salt);

            User user = _mapper.Map<User>(req);
            user.Password = encrypted;

            string token = CreateToken(user, 15);

            ConfirmToken userToken = new(token);
            user.ConfirmToken = userToken;
            if (req.Image?.Length > 0)
            {
                ImageUploadResult res = await UploadImageToCloud(req.Image);
                user.Image = res.Url.ToString();
                user.ImagePublicIP = res.PublicId.ToString();
            }

            var result = _applicationDbContext.Users.Add(user);
            _applicationDbContext.SaveChanges();

            return token;
        }

        public string DeleteUser(string auth)
        {
            string[] token = auth.Split(" ");
            string email = VerifyToken(token[1]);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email) ?? throw new Exception("User not found");
            _applicationDbContext.Remove(user);
            _applicationDbContext.SaveChanges();
            return "success";
        }

        public string ForgetPassword(ForgetPasswordRequest req)
        {
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == req.Email) ?? throw new Exception("User not found");
            string token = CreateToken(user, 15);
            ConfirmToken? ct = _applicationDbContext.ConfirmToken.FirstOrDefault(x => x.UserId == user.Id) ?? throw new Exception();
            ct.ForgetVerify = token;
            SendEmail(user.Email, "Confirm Email", user.Name, token);
            _applicationDbContext.ConfirmToken.Update(ct);
            _applicationDbContext.SaveChanges();
            return token;
        }

        public UserDto GetUser(string auth)
        {
            string[] token = auth.Split(" ");
            string email = VerifyToken(token[1]);
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == email) ?? throw new Exception("User not found");
            UserDto res = _mapper.Map<UserDto>(user);
            res.Password = "";
            return res;
        }

        public bool SendMail(string mail, string content)
        {
            throw new NotImplementedException();
        }

        public string UpdateUser(UpdateUserInfoRequest req)
        {
            User? user = _applicationDbContext.Users.FirstOrDefault(x => x.Email == req.Email);

            if (user == null)
                return "User not found";
            bool isCorrect = BCrypt.Net.BCrypt.Verify(req.OldPassword, user.Password);
            if (!isCorrect)
                return "Wrong password";
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var encrypted = BCrypt.Net.BCrypt.HashPassword(req.NewPassword, salt);
            user.Password = encrypted;
            user.Email = req.Email;
            user.Name = req.Name;
            user.Image = req.PhotoUrl;
            _applicationDbContext.Update(user);
            _applicationDbContext.SaveChanges();
            return "Success";
        }

        private string CreateToken(User user, int expire_min)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            var tokenKey = Encoding.ASCII.GetBytes(_configuration["ConnectionStrings:JWT_Token"]);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expire_min),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature) //setting sha256 algorithm
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var token = tokenHandler.WriteToken(securityToken);

            return token;
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
            catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }

        public void SendEmail(string to, string subject, string name, string token)
        {
            var link = "https://localhost:7052/api/user/confirmemail/token?token=" + token;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetConnectionString("Mail")));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = EmailTemplate(name, link) };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_configuration.GetConnectionString("Host"), 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetConnectionString("Mail"), _configuration.GetConnectionString("Password"));
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task<ImageUploadResult> UploadImageToCloud(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinaryDotNet.UploadAsync(uploadParams);
            }
            Console.WriteLine(uploadResult.PublicId.ToString());
            return uploadResult;
        }

        private static String EmailTemplate(String name, String link)
        {
            return "<div style=\"font-family:Helvetica,Arial,sans-serif;font-size:16px;margin:0;color:#0b0c0c\">\n" +
                    "\n" +
                    "<span style=\"display:none;font-size:1px;color:#fff;max-height:0\"></span>\n" +
                    "\n" +
                    "  <table role=\"presentation\" width=\"100%\" style=\"border-collapse:collapse;min-width:100%;width:100%!important\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">\n" +
                    "    <tbody><tr>\n" +
                    "      <td width=\"100%\" height=\"53\" bgcolor=\"#0b0c0c\">\n" +
                    "        \n" +
                    "        <table role=\"presentation\" width=\"100%\" style=\"border-collapse:collapse;max-width:580px\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\">\n" +
                    "          <tbody><tr>\n" +
                    "            <td width=\"70\" bgcolor=\"#0b0c0c\" valign=\"middle\">\n" +
                    "                <table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse\">\n" +
                    "                  <tbody><tr>\n" +
                    "                    <td style=\"padding-left:10px\">\n" +
                    "                  \n" +
                    "                    </td>\n" +
                    "                    <td style=\"font-size:28px;line-height:1.315789474;Margin-top:4px;padding-left:10px\">\n" +
                    "                      <span style=\"font-family:Helvetica,Arial,sans-serif;font-weight:700;color:#ffffff;text-decoration:none;vertical-align:top;display:inline-block\">Confirm your email</span>\n" +
                    "                    </td>\n" +
                    "                  </tr>\n" +
                    "                </tbody></table>\n" +
                    "              </a>\n" +
                    "            </td>\n" +
                    "          </tr>\n" +
                    "        </tbody></table>\n" +
                    "        \n" +
                    "      </td>\n" +
                    "    </tr>\n" +
                    "  </tbody></table>\n" +
                    "  <table role=\"presentation\" class=\"m_-6186904992287805515content\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse;max-width:580px;width:100%!important\" width=\"100%\">\n" +
                    "    <tbody><tr>\n" +
                    "      <td width=\"10\" height=\"10\" valign=\"middle\"></td>\n" +
                    "      <td>\n" +
                    "        \n" +
                    "                <table role=\"presentation\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse\">\n" +
                    "                  <tbody><tr>\n" +
                    "                    <td bgcolor=\"#1D70B8\" width=\"100%\" height=\"10\"></td>\n" +
                    "                  </tr>\n" +
                    "                </tbody></table>\n" +
                    "        \n" +
                    "      </td>\n" +
                    "      <td width=\"10\" valign=\"middle\" height=\"10\"></td>\n" +
                    "    </tr>\n" +
                    "  </tbody></table>\n" +
                    "\n" +
                    "\n" +
                    "\n" +
                    "  <table role=\"presentation\" class=\"m_-6186904992287805515content\" align=\"center\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse;max-width:580px;width:100%!important\" width=\"100%\">\n" +
                    "    <tbody><tr>\n" +
                    "      <td height=\"30\"><br></td>\n" +
                    "    </tr>\n" +
                    "    <tr>\n" +
                    "      <td width=\"10\" valign=\"middle\"><br></td>\n" +
                    "      <td style=\"font-family:Helvetica,Arial,sans-serif;font-size:19px;line-height:1.315789474;max-width:560px\">\n" +
                    "        \n" +
                    "            <p style=\"Margin:0 0 20px 0;font-size:19px;line-height:25px;color:#0b0c0c\">Hi " + name + ",</p><p style=\"Margin:0 0 20px 0;font-size:19px;line-height:25px;color:#0b0c0c\"> Thank you for registering. Please click on the below link to activate your account: </p><blockquote style=\"Margin:0 0 20px 0;border-left:10px solid #b1b4b6;padding:15px 0 0.1px 15px;font-size:19px;line-height:25px\"><p style=\"Margin:0 0 20px 0;font-size:19px;line-height:25px;color:#0b0c0c\"> <a href=\"" + link + "\">Activate Now</a> </p></blockquote>\n Link will expire in 15 minutes. <p>See you soon</p>" +
                    "        \n" +
                    "      </td>\n" +
                    "      <td width=\"10\" valign=\"middle\"><br></td>\n" +
                    "    </tr>\n" +
                    "    <tr>\n" +
                    "      <td height=\"30\"><br></td>\n" +
                    "    </tr>\n" +
                    "  </tbody></table><div class=\"yj6qo\"></div><div class=\"adL\">\n" +
                    "\n" +
                    "</div></div>";
        }
    }
}

using Business_Logic_Services;
using DTO;
using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Models;
using NuGet.Common;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace Hotel_Booking.Controllers
{
    public class AccountController : MVCControlBase
    {
        private readonly IConfiguration _config;
        private readonly EmailSender _emailSender;

        public AccountController(IUnitOfWork unitOfWork, IConfiguration config, EmailSender emailSender) : base(unitOfWork)
        {
            _config = config;
            _emailSender = emailSender;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            if (ModelState.IsValid)
            {
                var emailCount = _unitOfWork.Users.Count(u => u.Email == signUpDto.Email);
                
                if(emailCount > 0)
                {
                    ModelState.AddModelError("Email", "This email is already existing");
                    return View(signUpDto);
                }

                string encryptPassword=new PasswordHasher<User>().HashPassword(new User(), signUpDto.Password);

                var user = new User()
                {
                    FirstName = signUpDto.FirstName,
                    LastName = signUpDto.LastName,
                    Email = signUpDto.Email,
                    PhoneNumber = signUpDto.PhoneNumber,
                    Password = encryptPassword,
                    RegisterationDate = DateTime.Now,
                    Role = "Guest"
                };

                await _unitOfWork.Users.AddAsync(user);
                _unitOfWork.Save();

                return RedirectToAction("SignIn");
            }
            return View(signUpDto);
        }

        [AllowAnonymous]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInDto signInDto)
        {
            if (ModelState.IsValid)
            {
                var user=_unitOfWork.Users.FindOneItem(u=>u.Email == signInDto.Email);

                if(user != null)
                {
                    try
                    {
                        var found = new PasswordHasher<User>().VerifyHashedPassword(new User(), user.Password, signInDto.Password);

                        if (found == PasswordVerificationResult.Success)
                        {
                            var claims = new List<Claim>();
                            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                            claims.Add(new Claim("id", "" + user.Id));
                            claims.Add(new Claim("role", user.Role));
                            claims.Add(new Claim("name", user.FirstName+" "+user.LastName));

                            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecritKey"]!));
                            var signingCredentials = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                issuer: _config["JWT:Issuer"],
                                audience: _config["JWT:Audience"],
                                expires: DateTime.Now.AddMinutes(30),
                                claims: claims,
                                signingCredentials: signingCredentials
                            );

                            var finalToken = new JwtSecurityTokenHandler().WriteToken(token);

                            HttpContext.Response.Cookies.Append("jwt", finalToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = DateTime.Now.AddMinutes(30),
                                SameSite = SameSiteMode.Strict
                            });

                            //Sign In for Admin
                            if(user.Role == "Admin")
                            {
                                return RedirectToAction("Dashboard", "AdminRole");
                            }

                            return RedirectToAction("Index", "Hotel");
                        }
                    }
                    catch (Exception ex) 
                    {
                        ModelState.AddModelError("Password", ex.Message);
                    }

                    
                    ModelState.AddModelError("Password", "Wrong Password");
                    return View(signInDto);
                }
                ModelState.AddModelError("Email", "This email isn't found in the system");
            }
            return View(signInDto);
        }

        public IActionResult GetUserProfile()
        {
            int id = GetUserId(HttpContext);

            if (id != 0)
            {
                var user=_unitOfWork.Users.FindOneItem(u=>u.Id == id);

                return View(user);
            }
            return RedirectToAction("SignIn");
        }

        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            if(ModelState.IsValid)
            {
                var userId=GetUserId(HttpContext);

                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if(user != null)
                {
                    user.FirstName = userDto.FirstName;
                    user.LastName = userDto.LastName;
                    user.Email = userDto.Email;
                    user.PhoneNumber = userDto.PhoneNumber;

                    _unitOfWork.Save();
                    return RedirectToAction("GetProfileHistory", "UserHistory");
                }
            }
            return RedirectToAction("GetUserProfile");
        }

        [Authorize]
        public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword)
        {
            if(ModelState.IsValid)
            {
                int userId=GetUserId(HttpContext);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                var found = new PasswordHasher<User>().VerifyHashedPassword(new User(), user.Password, oldPassword);

                if (found ==PasswordVerificationResult.Success)
                {
                    var secondPassword = new PasswordHasher<User>().HashPassword(new User(), newPassword);
                    user.Password = secondPassword;

                    _unitOfWork.Save();

                    return RedirectToAction("GetProfileHistory", "UserHistory");
                }
            }
            return RedirectToAction("GetUserProfile");
        }

        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var user=_unitOfWork.Users.FindOneItem(u=>u.Email == forgetPasswordDto.Email);
                
                if(user == null)
                {
                    ModelState.AddModelError("Email", "This email isn't found in the system!");
                    return View(forgetPasswordDto);
                }

                var oldPassword=_unitOfWork.ResetPasswords.FindOneItem(r=>r.Email == forgetPasswordDto.Email);

                if (oldPassword != null)
                {
                    _unitOfWork.ResetPasswords.Delete(oldPassword);
                }

                //Generate Random Code to reset password
                Random random = new Random();

                string code=random.Next(100000,1000000).ToString();

                var resetPassword = new ResetPasswords()
                {
                    Email = forgetPasswordDto.Email,
                    Token = code,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ResetPasswords.AddAsync(resetPassword);

                _unitOfWork.Save();

                string subject = "Password Reset";
                string message = "Dear sir, " + "\n" +
                    "We received your password reset request.\n" +
                    "Please copy the following token and paste it in the Password Reset Form:\n" +
                code + "\n\n" +
                "Best Regards\n";

                _emailSender.SendEmail(subject, user.Email, message);
                return RedirectToAction("ResetPassword");
            }
            return View(forgetPasswordDto);
        }

        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var resetPassword=_unitOfWork.ResetPasswords.FindOneItem(r=>r.Token == resetPasswordDto.Token);

                if(resetPassword != null)
                {
                    var user = _unitOfWork.Users.FindOneItem(u => u.Email == resetPassword.Email);

                    if(user != null)
                    {
                        var encryptedPassword = new PasswordHasher<User>().HashPassword(new User(), resetPasswordDto.Password);

                        user.Password = encryptedPassword;

                        _unitOfWork.ResetPasswords.Delete(resetPassword);

                        _unitOfWork.Save();

                        return RedirectToAction("SignIn");
                    }
                }
                ModelState.AddModelError("Token", "Wrong Token");
            }
            return View(resetPasswordDto);
        }

        [Authorize]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("SignIn");
        }


        #region Get User Data From JWT in static methods

        //Get The all claims from jwt
        private static string? GetClaimFromJWT(HttpContext httpContext, string claimType)
        {
            var jwt = httpContext.Request.Cookies["jwt"];

            if (jwt == null)
            {
                return null;
            }

            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

            return token.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
        //Get User Id from GetClaimFromJwt
        public static int GetUserId(HttpContext httpContext)
        {
            string userId = GetClaimFromJWT(httpContext, "id")!;
            int id;
            try
            {
                id = int.Parse(userId);
            }
            catch (Exception)
            {
                return 0;
            }

            return id;
        }

        //Get User Id from GetClaimFromJwt
        public static string? GetUserName(HttpContext httpContext)
        {
            return GetClaimFromJWT(httpContext, "name")!;
        }

		public static string GetUserRole(HttpContext httpContext)
		{
			return GetClaimFromJWT(httpContext, "role")!;
		}
		#endregion

	}
}

using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using IdentityExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using NETCore.MailKit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Web;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public HomeController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string userId, string code)
        {
            //?a=foo&b=bar
            var query = new QueryBuilder();
            query.Add("userId", userId);
            query.Add("code", code);
            return View(model: query.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction(nameof(Secret));//nameof(Index));
                }
            }
            else
            {
                return BadRequest("This User does not exist");
            }
            return BadRequest("Something Went Wrong");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var user = new IdentityUser()
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //Url.Action("VerifyEmail", "Home", new { userId = user.Id, code },"http","localhost");

                string link = $"http://localhost:9557/Home/VerifyEmail?userId=" +
                    HttpUtility.UrlEncode(user.Id) +
                    "&code=" + HttpUtility.UrlEncode(code) +
                    "&Scheme=" + HttpUtility.UrlEncode(HttpContext.Request.Scheme);

                await _emailSender.SendEmail("System", "system@test.com", user.UserName, user.Email,
                    "Email Verification", $"Please verify your email using this <a href=\"{link}\">link</a>", "localhost", 25);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction(nameof(EmailVerification));
        }

        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return View();
            }
            return BadRequest("Omo I no sabi");
        }

        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userPrincipal = HttpContext.User; //await _userManager.FindByNameAsync(username);
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
            {
                await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("No user with this email exists");
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            string link = $"http://localhost:9557/Home/ResetPassword?userId=" +
                    HttpUtility.UrlEncode(user.Id) +
                    "&code=" + HttpUtility.UrlEncode(code) +
                    "&Scheme=" + HttpUtility.UrlEncode(HttpContext.Request.Scheme);

            await _emailSender.SendEmail("System", "system@test.com", user.UserName, user.Email,
                "Forgot Password", $"Seems you have forgoten your password, to reset your password please use this <a href=\"{link}\">link</a>", "localhost", 25);

            return Ok("Please check your email for the password reset link");
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId, string code, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("No user with this email exists");
            }

            var result = await _userManager.ResetPasswordAsync(user,code, newPassword);

            if (result.Succeeded)
            {
                return Ok("Password reset ");
            }

            return BadRequest("Something went Wrong");
        }
    }
}

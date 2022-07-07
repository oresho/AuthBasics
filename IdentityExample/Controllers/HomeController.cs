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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user,password,false,false);
                if (signInResult.Succeeded)
                {
                    return RedirectToAction(returnUrl);//nameof(Index));
                }
            }
            else
            {
                return Content("This User does not exist");
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser()
            {
                UserName = username,
                Email = ""
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //Url.Action("VerifyEmail", "Home", new { userId = user.Id, code });

                string link = $"http://localhost:9557/Home/VerifyEmail?userId=" +
                    HttpUtility.UrlEncode(user.Id) +
                    "&code=" + HttpUtility.UrlEncode(code) +
                    "&Scheme=" + HttpUtility.UrlEncode(HttpContext.Request.Scheme);

                await _emailSender.SendEmail("System", "system@test.com", "Ore", "Ore@test.com",
                    "Email Verification",$"Please verify your email using this <a href=\"{link}\">link</a>", "localhost", 25);
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
                return Content("User not found");//BadRequest();
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
    }
}

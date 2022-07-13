using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationBasics.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationBasics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public HomeController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
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

        [Authorize(Policy = "Claims.DoB")]
        public IActionResult SecretPolicy()
        {
            return RedirectToAction("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return RedirectToAction("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return RedirectToAction("Secret");
        }

        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return RedirectToAction("Secret");
        }

        public IActionResult Authenticate()
        {
            var OreClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ore"),
                new Claim(ClaimTypes.Email, "Ore@gmail.com"),
                new Claim(ClaimTypes.DateOfBirth, "11/11/2011"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(DynamicPolicies.SecurityLevel, "7"),
                new Claim("Ore says", "Ore"),
            };
            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Ore Sho"),
                new Claim("Driving License", "A+"),
            };

            var OreIdentity = new ClaimsIdentity(OreClaims, "Ore Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { OreIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DOstuff([FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

            //var auth = await _authorizationService.AuthorizeAsync(User, customPolicy); You can do it like this using DI globally
            var auth = await authorizationService.AuthorizeAsync(User, customPolicy);//or like this using DI in this method alone

            if (auth.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}

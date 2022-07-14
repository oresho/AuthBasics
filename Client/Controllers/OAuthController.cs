using System;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class OAuthController : Controller
    {
        public OAuthController()
        {
        }

        [HttpGet]
        public IActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authorize(string username)
        {
            return View();
        }

        public IActionResult Token()
        {
            return View();
        }
    }
}

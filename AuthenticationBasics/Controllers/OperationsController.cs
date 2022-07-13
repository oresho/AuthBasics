using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationBasics.Controllers
{
    public class OperationsController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationsController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar();//resource that needs to be used
            //var requirement = new OperationAuthorizationRequirement()//requirement to perform action on the resource
            //{
            //    Name = CookieJarOperations.Open
            //};
            await _authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.Open);//authoriztionservice to check if this user is permitted to carry out this action on the resource
            return View();
        }
    }

    public class CookieJarAuthorizationHandler :
        AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJar cookieJar)
        {
            if (requirement.Name == CookieJarOperations.Look)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    //if the user is auth he can view my cookies
                    context.Succeed(requirement);
                }
            }
            if (requirement.Name == CookieJarOperations.ComeNear)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }

        public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string Look = "Look";
        public static string ComeNear = "ComeNear";
        public static string TakeCookie = "TakeCookie";
    }

    public class CookieJar
    {
        public string Name { get; set; }
    }
}

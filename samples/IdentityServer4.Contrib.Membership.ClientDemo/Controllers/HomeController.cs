namespace IdentityServer4.Contrib.Membership.ClientDemo.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View(new HelloResponse { Result = "Many bothans died to bring you this information." });
        }
    }
}

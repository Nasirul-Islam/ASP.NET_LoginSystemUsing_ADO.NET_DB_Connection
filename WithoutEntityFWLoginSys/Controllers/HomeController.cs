using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;
using WithoutEntityFWLoginSys.Models;

namespace WithoutEntityFWLoginSys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            // Validate the credentials (this is a basic example)
            if (Username == "admin" && Password == "12345")
            {
                // Create claims that represent the user
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, "Admin") // You can use roles too
            };

                // Create identity and principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign the user in with the authentication scheme
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);



                // Store the credentials in TempData, which survives across redirects
                TempData["Username"] = Username;
                TempData["Password"] = Password;
                // If credentials are valid, redirect to some other action
                return RedirectToAction("Dashboard");
                // in this method show data in url
                //return RedirectToAction("Dashboard", new { Username, Password });
            }

            // If invalid, stay on the same page and show an error message (or other logic)
            ViewBag.Error = "Invalid username or password";
            return View(); // Assuming the login form is on the Index view
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            //string Username, string Password
            // Retrieve the Username and Password from TempData
            string? username = TempData["Username"] as string;
            string? password = TempData["Password"] as string;  
            // Avoid displaying this, use it only if needed

            ViewBag.name = username;
            ViewBag.pass = password;
            return View(); // You can create this view for a successful login page
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using System.Security.Claims;
using Hospital_Management.Data;
using Hospital_Management.Filters;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HealthCareManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly HealthCareDbContext _context;



        public AccountController(HealthCareDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                email = email.Trim();
                password = password.Trim();

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == (email.ToLower()) && u.Password == (password));

                

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.UserType),
                        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    var userType = _context.Users
                        .Where(u => u.Email == email)
                        .Select(u => u.UserType)
                        .FirstOrDefault();

                    HttpContext.Session.SetString("UserType", userType);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View("AccessDenied");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.SetString("UserType", "");
            return RedirectToAction("Login", "Account");
        }
        
        public IActionResult Register()
        {
            return View();   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserID,Username,Password,Email,PhoneNumber,UserType")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    /*return RedirectToAction(nameof(Index));*/
                    TempData["Success"] = "User Created Successfully";
                    var usrType = HttpContext.Session.GetString("UserType");
                    if(usrType == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Something went wrong!!";
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View("Login");
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

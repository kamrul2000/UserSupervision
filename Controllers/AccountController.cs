using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UserSupervision.Models;
using UserSupervision.Data;

namespace UserSupervision.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.SupervisionTable
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            var subscriptionId = await GetDefaultSubscriptionId();

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Mobile = model.Mobile,
                Address = model.Address,
                RoleId = 1,
                BranchId = null,
                Status = true,
                SubscriptionId = subscriptionId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
               
            };

            _context.Users.Add(user); 

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        private async Task<int> GetDefaultSubscriptionId()
        {
            var subscription = await _context.Subscriptions
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .FirstOrDefaultAsync();

            if (subscription == null)
                throw new Exception("No active subscription found");

            return subscription.Id;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UserSupervision.Models;
using UserSupervision.Models.ViewModel;
using UserSupervision.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace UserSupervision.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccuStockDbContext _context;
        private readonly AppDbContext _appDbContext;
        


        public AccountController(AccuStockDbContext context, AppDbContext appDbContext)
        {
            _context = context;
            _appDbContext = appDbContext;
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            var user = await _appDbContext.SupervisionTable.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);


            if (user != null)
            {
                var claims = new List<Claim>
                {
                 new Claim(ClaimTypes.Name, user.Name),
                 new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = false
                });

                return RedirectToAction("Dashboard", "Home");
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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Account");
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserSupervision.Data;

namespace UserSupervision.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var usersWithNoBranch = await _context.Users
                .Where(u => u.BranchId == null)
                .ToListAsync();

            return View(usersWithNoBranch);
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserSupervision.Data;

namespace UserSupervision.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AccuStockDbContext _context;

        public DashboardController(AccuStockDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var usersWithNoBranch = await _context.Users
                .Where(u => u.BranchId == null)
                .ToListAsync();

                return View(usersWithNoBranch);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }


    }
}

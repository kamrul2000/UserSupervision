using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserSupervision.Data;

namespace UserSupervision.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AccuStockDbContext _context;
        private readonly AppDbContext _appDbContext;


        public DashboardController(AccuStockDbContext context, AppDbContext appDbContext)
        {
            _context = context;
            _appDbContext = appDbContext;
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

        [Authorize]
        public async Task<IActionResult> BillIndex()
        {
            var user = await _context.Users
            .Where(u => u.BranchId == null)           
            .ToListAsync();
            return View(user);
        }


    }
}

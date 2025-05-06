using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserSupervision.Data;
using UserSupervision.Models.ViewModel;
using UserSupervision.Models;

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
            var usersWithCompanies = await _context.Users
                .Where(u => u.BranchId == null)
                .Select(u => new UserWithCompanyViewModel
                {
                    User = u,
                    CompanyName = _context.Companies
                        .Where(c => c.SubscriptionId == u.SubscriptionId)
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(usersWithCompanies);
        }



    }
}

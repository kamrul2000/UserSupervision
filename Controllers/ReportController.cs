using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserSupervision.Data;
using UserSupervision.Models.ViewModel;

namespace UserSupervision.Controllers
{
    public class ReportController : Controller
    {
        private readonly AccuStockDbContext _context;
        private readonly AppDbContext _appDbContext;

        public ReportController(AccuStockDbContext context, AppDbContext appDbContext)
        {
            _context = context;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> ShowReport(string statusFilter = "", string companySearch = "")
        {
            var billsQuery = _appDbContext.UserBillStatuses.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
                billsQuery = billsQuery.Where(b => b.PaymentStatus == statusFilter);

            var bills = await billsQuery.ToListAsync();

            var userIds = bills.Select(b => b.UserId).Distinct().ToList();
            var subscriptionIds = bills.Select(b => b.SubscriptionId).Distinct().ToList();

            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var companies = await _context.Companies
                .Where(c => subscriptionIds.Contains(c.SubscriptionId))
                .ToListAsync();

            var result = bills.Select((bill, index) =>
            {
                var user = users.FirstOrDefault(u => u.Id == bill.UserId);
                var company = companies.FirstOrDefault(c => c.SubscriptionId == bill.SubscriptionId);

                return new ReportViewModel
                {
                    SLNo = index + 1,
                    UserName = user?.FullName ?? "N/A",
                    Amount = bill.Amount,
                    FromDate = bill.FromDate,
                    ToDate = bill.DueDate,
                    CompanyName = company?.Name ?? "N/A",
                    PaymentStatus = bill.PaymentStatus
                };
            }).ToList();

            if (!string.IsNullOrEmpty(companySearch))
                result = result.Where(r => r.CompanyName.Contains(companySearch, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Statuses = await _appDbContext.UserBillStatuses
                .Select(b => b.PaymentStatus)
                .Distinct()
                .ToListAsync();

            return View(result);
        }
    }
}

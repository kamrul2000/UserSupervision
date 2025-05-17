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
                    .Where(u => u.BranchId == null && u.RoleId == 1)
                    .ToListAsync();

                return View(usersWithNoBranch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        [Authorize]
        public async Task<IActionResult> BillIndex()
        {
            var users = await _context.Users
                .Where(u => u.BranchId == null && u.RoleId == 1)
                .Select(u => new
                {
                User = u,
                CompanyName = _context.Companies
                .Where(c => c.SubscriptionId == u.SubscriptionId)
                .Select(c => c.Name)
                .FirstOrDefault()
                })
                .ToListAsync();

                            var userBillStatuses = await _appDbContext.UserBillStatuses
                                .ToListAsync();

            var usersWithCompanies = users.Select(u =>
            {
                var latestBill = userBillStatuses
                    .Where(b => b.SubscriptionId == u.User.SubscriptionId && b.UserId == u.User.Id)
                    .OrderByDescending(b => b.BillDate)
                    .FirstOrDefault();

                return new UserWithCompanyViewModel
                {
                    User = u.User,
                    CompanyName = u.CompanyName,
                    PaymentStatus = latestBill?.PaymentStatus ?? "Unpaid",
                    Amount = latestBill?.Amount ?? 0 
                };
            }).ToList();

            return View(usersWithCompanies);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveBill(int userId, decimal amount, DateTime fromDate, DateTime toDate, int subscriptionId)
        {
            try
            {
                var previousDueBills = await _appDbContext.UserBillStatuses
                    .Where(b => b.UserId == userId &&
                                b.SubscriptionId == subscriptionId &&
                                b.PaymentStatus == "Due")
                    .ToListAsync();

                foreach (var bill in previousDueBills)
                {
                    bill.PaymentStatus = "Unpaid";
                }

                var newBill = new UserBillStatus
                {
                    UserId = userId,
                    SubscriptionId = subscriptionId,
                    Amount = amount,
                    BillDate = DateTime.Now,
                    DueDate = toDate,
                    PaymentStatus = "Due",
                    InvoiceNumber = $"B-{subscriptionId}{DateTime.Now:dd}{DateTime.Now:MM}{DateTime.Now:yyyy}"
                };

                _appDbContext.UserBillStatuses.Add(newBill);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("BillIndex");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("BillIndex"); 

            }
        }

        public async Task<IActionResult> BillPay()
        {
            var bills = await _appDbContext.UserBillStatuses
                .Where(b => b.PaymentStatus == "Due" || b.PaymentStatus == "Unpaid")
                .ToListAsync();

            return View(bills);
        }

        [HttpPost]
        public async Task<IActionResult> PayBill(BillPayViewModel model)
        {
            var bill = await _appDbContext.UserBillStatuses
                .FirstOrDefaultAsync(b => b.InvoiceNumber == model.InvoiceNumber);

            if (bill == null)
            {
                ModelState.AddModelError("", "Bill not found");
                return View("BillPay");
            }

            var billPay = new BillPay
            {
                InvoiceNumber = model.InvoiceNumber,
                SubscriptionId = model.SubscriptionId,
                AmountPaid = model.AmountPaid,
                MoneyReceiptOrCheckNumber = model.MoneyReceiptOrCheckNumber,
                Status = model.AmountPaid >= bill.Amount ? "Paid" : "Partial Paid",
                PaymentDate = DateTime.Now
            };

            bill.PaymentStatus = billPay.Status;

            _appDbContext.BillPays.Add(billPay);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("BillPay");
        }





    }
}

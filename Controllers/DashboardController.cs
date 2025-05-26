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
                    .Where(b => b.SubscriptionId == u.User.SubscriptionId
                             && b.UserId == u.User.Id
                             && b.PaymentStatus != "Paid")
                    .OrderByDescending(b => b.BillDate)
                    .FirstOrDefault();

                return new UserWithCompanyViewModel
                {
                    User = u.User,
                    CompanyName = u.CompanyName,
                    PaymentStatus = latestBill?.PaymentStatus ?? "",  
                    Amount = latestBill?.Amount ?? 0
                };
            }).ToList();


            return View(usersWithCompanies);
        }

        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> SaveBill(int userId, decimal amount, DateTime fromDate, DateTime toDate, int subscriptionId)
        {
            try
            {
                var existingBill = await _appDbContext.UserBillStatuses
                    .Where(b => b.UserId == userId &&
                                b.SubscriptionId == subscriptionId &&
                                (b.PaymentStatus == "Due" || b.PaymentStatus == "Partial Paid"))
                    .OrderByDescending(b => b.BillDate)
                    .FirstOrDefaultAsync();

                if (existingBill != null)
                {
                    existingBill.Amount = amount;
                    existingBill.BillDate = DateTime.Now;
                    existingBill.DueDate = toDate;
                    existingBill.FromDate = fromDate; 
                }
                else
                {
                    var newBill = new UserBillStatus
                    {
                        UserId = userId,
                        SubscriptionId = subscriptionId,
                        Amount = amount,
                        BillDate = DateTime.Now,
                        DueDate = toDate,
                        FromDate = fromDate, 
                        PaymentStatus = "Due",
                        InvoiceNumber = $"B-{subscriptionId}-{userId}-{DateTime.Now:yyyyMMddHHmmss}"
                    };

                    _appDbContext.UserBillStatuses.Add(newBill);
                }

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
                .Where(b => b.PaymentStatus != "Paid")
                .ToListAsync();

            var payments = await _appDbContext.BillPays.ToListAsync();

            var viewModel = bills.Select(bill =>
            {
                var totalPaid = payments
                    .Where(p => p.InvoiceNumber == bill.InvoiceNumber)
                    .Sum(p => p.AmountPaid);

                return new BillPayViewModel
                {
                    InvoiceNumber = bill.InvoiceNumber,
                    SubscriptionId = bill.SubscriptionId,
                    BillAmount = bill.Amount,
                    AmountPaid = totalPaid,
                    MoneyReceiptOrCheckNumber = payments
                        .Where(p => p.InvoiceNumber == bill.InvoiceNumber)
                        .OrderByDescending(p => p.PaymentDate)
                        .Select(p => p.MoneyReceiptOrCheckNumber)
                        .FirstOrDefault()
                };
            }).ToList();

            return View(viewModel); 
        }


        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> PayBill(BillPayViewModel model)
        {
            var bill = await _appDbContext.UserBillStatuses
                .FirstOrDefaultAsync(b => b.InvoiceNumber == model.InvoiceNumber);

            if (bill == null)
            {
                ModelState.AddModelError("", "Bill not found");
                return RedirectToAction("BillPay");
            }

            var newPayment = new BillPay
            {
                InvoiceNumber = model.InvoiceNumber,
                SubscriptionId = model.SubscriptionId,
                AmountPaid = model.AmountPaid,
                MoneyReceiptOrCheckNumber = model.MoneyReceiptOrCheckNumber,
                Status = model.AmountPaid + _appDbContext.BillPays
                             .Where(p => p.InvoiceNumber == model.InvoiceNumber)
                             .Sum(p => p.AmountPaid)
                         >= bill.Amount ? "Paid" : "Partial Paid",
                PaymentDate = DateTime.Now
            };

            _appDbContext.BillPays.Add(newPayment);

            var totalPaid = await _appDbContext.BillPays
                .Where(p => p.InvoiceNumber == model.InvoiceNumber)
                .SumAsync(p => p.AmountPaid);

            bill.PaymentStatus = (totalPaid + model.AmountPaid) >= bill.Amount ? "Paid" : "Partial Paid";
            _appDbContext.UserBillStatuses.Update(bill);

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("BillPay");
        }







    }
}

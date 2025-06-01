using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserSupervision.Models;

namespace UserSupervision.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SupervisionUser> SupervisionTable { get; set; }
        public DbSet<UserBillStatus> UserBillStatuses { get; set; }
        public DbSet<BillPay> BillPays { get; set; }
        public DbSet<Menu> Menus { get; set; }


    }
}
